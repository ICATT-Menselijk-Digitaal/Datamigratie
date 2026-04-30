# Questions for OpenZaak — Performance & Bug Report

## Background

We are migrating case files from E-Suite (DET) to OpenZaak using an automated migration tool. For each case, the following objects are created via the OpenZaak REST API:

- Zaak
- Zaak roles
- Zaak statuses
- Zaak result
- Enkelvoudige informatieobjecten (documents)
- Zaakinformatieobjecten (zaak ↔ document links)
- Zaak decisions (besluiten)

During a re-migration, existing objects are deleted before being recreated.

---

## 1. Performance

### 1.1 API call volume per zaak

The migration process makes a significant number of sequential API calls per zaak. Below is an estimate based on the migration steps, where **D** = number of documents and **V** = average number of versions per document:

**Re-migration cleanup (if zaak already exists):**
| Step | Calls |
|------|-------|
| `GET /zaken?identificatie=...` | 1 |
| `GET /zaakinformatieobjecten?zaak=...` | 1 |
| `GET /besluiten?zaak=...` | 1 |
| `DELETE /besluiten/{id}` | per besluit |
| `DELETE /zaken/{id}` | 1 |
| `DELETE /enkelvoudiginformatieobjecten/{id}` | D total |

**Migration (create):**
| Step | Calls |
|------|-------|
| `POST /zaken` | 1 |
| `POST /resultaten` | 1 |
| `POST /statussen` | 1 |
| **PDF document** | 4 (create, upload, unlock, koppel) |
| **First version per document** | 5 per document (create, DET fetch, upload, unlock, koppel) |
| **Additional versions per document** | 7 per version (lock, update, GET, DET fetch, upload, unlock, koppel) |
| `POST /rollen` | per rol |
| `POST /besluiten` | per besluit |

**Total OpenZaak calls:** roughly `8 + D*5 + (V-1)*D*7` per zaak.

For example: a zaak with 3 documents each having 2 versions: `8 + 3*5 + 1*3*7 = 44 calls`.

This means the OpenZaak API call volume scales steeply with document count and version count.

### 1.2 What is the recommended maximum parallelism for write operations?

We use `Parallel.ForEachAsync` with a configurable `MaxDegreeOfParallelism` to migrate multiple zaken simultaneously. Within a single zaak, all steps are sequential. We observe that at higher parallelism levels, response times increase significantly.

#### Observed measurements

The table below shows elapsed time for actual migration runs against the test environment

**Zaaktype A** — 13 zaken, small size documents

| Concurrency | Processed | Succeeded | Failed | Elapsed |
| :---------: | :-------: | :-------: | :----: | ------: |
|      5      |   13/13   |    11     |   2    |     35s |
|      1      |   13/13   |    11     |   2    |     50s |

**Zaaktype B** — 396 zaken, ~390 document uploads

| Concurrency | Processed | Succeeded | Failed | Uploads | Elapsed |
| :---------: | :-------: | :-------: | :----: | :-----: | ------: |
|      1      |  396/396  |    385    |   11   |   390   | 22m 23s |
|      5      |  396/396  |    386    |   10   |   390   | 23m 40s |
|      3      |  396/396  |    385    |   11   |   389   | 21m 52s |

→ Concurrency 5 is marginally _slower_ than concurrency 1.

**Per-operation timing breakdown for Zaaktype B** (accumulated across all workers, ~390 uploads)

| Operation                 | Concurrency 1 total | Concurrency 1 avg/upload | Concurrency 3 total | Concurrency 3 avg/upload |           Ratio |
| ------------------------- | ------------------: | -----------------------: | ------------------: | -----------------------: | --------------: |
| Upload (`UploadBestand`)  |            51,063ms |                    131ms |           114,268ms |                    294ms | **2.2×** slower |
| Unlock (`UnlockDocument`) |            92,904ms |                    238ms |           187,974ms |                    483ms | **2.0×** slower |
| Koppel (`KoppelDocument`) |           110,491ms |                    283ms |           370,558ms |                    953ms | **3.4×** slower |

**Zaaktype C** — 281 zaken, 113 succeeded, 168 failed, 113 document uploads

| Concurrency | Processed | Succeeded | Failed | Uploads | Elapsed |
| :---------: | :-------: | :-------: | :----: | :-----: | ------: |
|      1      |  281/281  |    113    |  168   |   113   | 15m 01s |
|      3      |  281/281  |    113    |  168   |   113   | 14m 33s |

→ Concurrency 3 saves only ~28 seconds (−3%) versus concurrency 1 — effectively no improvement. The 60% failure rate is a separate issue unrelated to concurrency

**Per-operation timing breakdown for Zaaktype C** (accumulated across all workers, 113 uploads)

| Operation                 | Concurrency 1 total | Concurrency 1 avg/upload | Concurrency 3 total | Concurrency 3 avg/upload |           Ratio |
| ------------------------- | ------------------: | -----------------------: | ------------------: | -----------------------: | --------------: |
| Upload (`UploadBestand`)  |            14,952ms |                    132ms |            30,432ms |                    269ms | **2.0×** slower |
| Unlock (`UnlockDocument`) |            27,806ms |                    246ms |            48,841ms |                    432ms | **1.8×** slower |
| Koppel (`KoppelDocument`) |            41,098ms |                    364ms |           113,351ms |                  1,003ms | **2.8×** slower |

**Concurrency ≥ 5 — Circuit breaker risk**

When concurrency was increased to 5, we observed intermittent failures consistent with circuit breaker trips. At concurrency 10, the Polly circuit breaker opened on all HTTP clients consistently, halting the migration entirely:

```
BrokenCircuitException: The circuit is now open and is not allowing calls.
```

The migration tool uses a circuit breaker as a safety mechanism against overload; once open, all subsequent calls fail immediately until the circuit recovers. Based on our measurements, **concurrency 5 or higher carries a significant risk of triggering this behaviour.**

---

#### Analysis

##### What the data shows

The measurements show a consistent pattern across all document-heavy runs: **increasing concurrency does not reduce migration time, but it does increase per-request latency for every individual API call.**

Although it's expected that more parallel workers should do more work per second, the reason it doesn't is that certain OpenZaak endpoints can only handle one write at a time, so extra workers just end up waiting in line:

##### `KoppelDocument`

`POST /zaakinformatieobjecten` (linking a document to a zaak) is the slowest, and it degrades disproportionately fast:

- At concurrency 1 it takes **283–364ms** per call — already slower than the file upload itself.
- At concurrency 3 it takes **953–1003ms** per call — a 2.8–3.4× slowdown, almost exactly matching the concurrency factor.

A degradation ratio that closely tracks the number of workers is the hallmark of a **serialized resource**: the three workers are not truly running in parallel for this step — they are queuing behind each other. The most likely cause is a row-level lock or unique-constraint check on the `zaakinformatieobjecten` table in the OpenZaak database, which prevents two workers from writing to it simultaneously for the same zaak type or UUID namespace.

The result: at concurrency 3, the total accumulated koppel time is **3.4× higher** (370s vs 110s), but the total elapsed time is nearly the same, because the workers spend most of their time waiting for koppel to complete one at a time.

##### `UnlockDocument` is slower than the file upload

At concurrency 1 on both zaaktypes, `UnlockDocument` (238–246ms) takes **1.8–1.9× longer** than the actual file upload (131–132ms). This is unexpected: a `DELETE /enkelvoudiginformatieobjecten/{id}/unlock` request carries no file content, yet it takes almost twice as long as streaming a file to the server.

This suggests that `UnlockDocument` triggers significant server-side work on completion — Under concurrency, this overhead compounds: at concurrency 3, unlock degrades by 2.0× on Zaaktype B and 1.8× on Zaaktype C, again consistent with partial serialization.

##### Summary

| Finding                                                | Evidence                                                                     |
| ------------------------------------------------------ | ---------------------------------------------------------------------------- |
| `KoppelDocument` is serialized at the database level   | Degradation ratio matches concurrency factor exactly (2.8–3.4×)              |
| `UnlockDocument` triggers significant server-side work | 1.8–1.9× slower than file upload despite carrying no payload                 |
| Concurrency only helps when there are less documents   | Zaaktype A (no docs): 30% faster at c=5. Zaaktypes B & C (docs): 0–5% change |
| Concurrency 3 provides marginal benefit at best        | At most 5% total time improvement, sometimes with more failures              |
| Concurrency ≥ 5 is unsafe                              | Circuit breaker trips intermittently at c=5, reliably at c=10                |
| The bottleneck is server-side, not client-side         | Adding workers increases total API time, not throughput                      |

- What is the recommended `MaxDegreeOfParallelism` for write operations against the OpenZaak API?
- Is there a row-level or table-level lock on `zaakinformatieobjecten` during `POST` operations?
- Is there a rate limit or throttling policy active on the API?

### 1.3 Sequential constraint on document versions

For documents with multiple versions, our migration is sequential per document: each version requires a **lock → update → GET → upload → unlock** cycle before the next can start. This cannot be parallelized because OpenZaak only allows one active lock per document.

- Is there a more efficient API pattern for creating a document with multiple pre-existing versions in bulk?

### 1.4 Are there known performance bottlenecks when uploading large files?

We upload documents in parts (`bestandsdelen`). We observe variable response times for larger files.

- Is there a maximum number of concurrent uploads the server can handle?
- Does the lock/unlock pattern affect performance under parallel uploads?
