# Migration Flow

This document describes the technical flow of a single migration run, from start to finish.

---

## Overview

A migration run processes all zaken of a given zaaktype from DET (E-Suite) and creates the corresponding objects in OpenZaak. Each zaak is migrated independently. Multiple zaken are processed in parallel, with a configurable concurrency limit.

---

## Step 1 — Create Migration Record

A new migration record is created in the database with status `in_progress`.

```
DB → INSERT migrations (status: in_progress)
   ← migrationId
```

---

## Step 2 — Fetch Zaken from DET

The list of zaken to migrate is retrieved from DET based on the zaaktype.

### 2a. Full migration

All zaken of the zaaktype are retrieved:

```
DET → GET /zaaktype/{zaaktypeId}/zaken
    ← list of all zaken
```

### 2b. Partial migration

All zaken are retrieved, then zaken already successfully migrated are excluded:

```
DET → GET /zaaktype/{zaaktypeId}/zaken
    ← list of all zaken

DB  → SELECT successfully migrated zaken
    ← list of migrated zaaknummers

result = all zaken − already migrated zaken
```

---

## Step 3 — Update Total Record Count

The migration record in the database is updated with the total number of zaken to be processed.

```
DB → UPDATE migrations SET total_records = {count}
```

---

## Step 4 — Migrate Each Zaak (Parallel)

Each zaak in the list is migrated independently. Steps 4.1–4.10 below are executed **sequentially per zaak**, but multiple zaken run **in parallel** up to the configured concurrency limit.

---

### 4.1 — Fetch Zaak Details from DET

Full zaak details (including documents, besluiten, rollen) are fetched from DET.

```
DET → GET /zaken/{zaaknummer}
    ← DetZaak (with documents, versions, besluiten, rollen)
```

---

### 4.2 — Clean Up Existing Zaak in OpenZaak *(re-migration only)*

If a zaak with the same `identificatie` already exists in OpenZaak, it is deleted along with all related objects before proceeding. This enables idempotent re-migration.

**Check for existing zaak(en):**

```
OZ → GET /zaken/api/v1/zaken?identificatie__icontains={zaaknummer}
   ← list of matching zaken (0 or more)
```

If one or more zaken are found, for each:

**Fetch related objects:**

```
OZ → GET /zaken/api/v1/zaakinformatieobjecten?zaak={zaakUrl}
   ← list of zaak-document links

OZ → GET /besluiten/api/v1/besluiten?zaak={zaakUrl}
   ← list of besluiten
```

**Delete besluiten:**

```
OZ → DELETE /besluiten/api/v1/besluiten/{besluitId}    (repeated per besluit)
```

**Delete zaak:**

```
OZ → DELETE /zaken/api/v1/zaken/{zaakId}
```

**Delete linked documents:**

```
OZ → DELETE /documenten/api/v1/enkelvoudiginformatieobjecten/{documentId}    (repeated per document)
```

> **Note:** If multiple zaken with the same `identificatie` are found (e.g. due to a previous failed migration), all of them are deleted before proceeding.

---

### 4.3 — Create Zaak

```
OZ → POST /zaken/api/v1/zaken
   ← OzZaak (url, zaaktype, identificatie)
```

---

### 4.4 — Create Resultaat

```
OZ → POST /zaken/api/v1/resultaten
   ← OzResultaat
```

---

### 4.5 — Create Status

```
OZ → POST /zaken/api/v1/statussen
   ← OzStatus
```

---

### 4.6 — Create and Upload Zaakgegevens PDF

A PDF containing zaak metadata is generated and uploaded for every zaak.

```
OZ → POST /documenten/api/v1/enkelvoudiginformatieobjecten    (create document metadata)
   ← OzDocument (with bestandsdelen upload URL and lock token)

OZ → PUT  {upload_url}                                        (upload file content)

OZ → POST /documenten/api/v1/enkelvoudiginformatieobjecten/{id}/unlock

OZ → POST /zaken/api/v1/zaakinformatieobjecten                (link document to zaak)
```

---

### 4.7 — Migrate Documents

Each document from DET is migrated including all its versions.

#### First version

```
OZ  → POST /documenten/api/v1/enkelvoudiginformatieobjecten   (create document metadata)
    ← OzDocument (with bestandsdelen upload URL and lock token)

DET → GET  /documenten/inhoud/{id}                            (fetch file content)
    ← file stream

OZ  → PUT  {upload_url}                                       (upload file content)

OZ  → POST /documenten/api/v1/enkelvoudiginformatieobjecten/{id}/unlock

OZ  → POST /zaken/api/v1/zaakinformatieobjecten               (link document to zaak)
```

#### Each additional version

```
OZ  → POST /documenten/api/v1/enkelvoudiginformatieobjecten/{id}/lock
    ← lockToken

OZ  → PUT  /documenten/api/v1/enkelvoudiginformatieobjecten/{id}    (update metadata for new version)

OZ  → GET  /documenten/api/v1/enkelvoudiginformatieobjecten/{id}    (fetch updated document with new bestandsdelen)
    ← OzDocument (with new upload URL)

DET → GET  /documenten/inhoud/{id}                                  (fetch file content for this version)
    ← file stream

OZ  → PUT  {upload_url}                                             (upload file content)

OZ  → POST /documenten/api/v1/enkelvoudiginformatieobjecten/{id}/unlock

OZ  → POST /zaken/api/v1/zaakinformatieobjecten                     (link version to zaak)
```

> **Note:** Document versions are migrated strictly sequentially per document because OpenZaak only permits one active lock at a time per document.

---

### 4.8 — Migrate Besluiten

```
OZ → POST /besluiten/api/v1/besluiten    (repeated per besluit)
```

---

### 4.9 — Migrate Rollen

```
OZ → POST /zaken/api/v1/rollen    (repeated per rol)
```

---

### 4.10 — Mark Zaak as Migrated in DET

Once all objects are successfully created in OpenZaak, the zaak is flagged as migrated in DET.

```
DET → PATCH /zaken/{functioneleIdentificatie}
      body: { "gemigreerd": true }
```

---

## Step 5 — Write Result to Database

After each zaak completes (success or failure), a `MigrationRecord` is written to the database. A single dedicated consumer thread handles all database writes to avoid concurrency issues.

```
DB → INSERT migration_records (zaaknummer, status, error_details, ...)
DB → UPDATE migrations SET processed_records++, successful_records++ / failed_records++
```

---

## Step 6 — Complete Migration

Once all zaken have been processed, the migration record is updated to its final status.

```
DB → UPDATE migrations SET status = completed | cancelled | failed, completed_at = now()
```

---

## Notes

| Topic | Detail |
|---|---|
| Parallelism | Zaken are processed in parallel. Concurrency is controlled by `ZaakConcurrencyLimit` (env: `Migration__ZaakConcurrencyLimit`). |
| Idempotency | Existing zaken and their related objects are deleted before re-creation, making each migration run safe to retry. |
| DB writes | All database writes are handled by a single consumer thread using a `Channel<MigrationRecord>` to prevent concurrent EF Core context access. |
| Document versions | Versions are migrated sequentially per document due to the OpenZaak lock constraint. |
| Cancellation | The migration can be cancelled at any time. Partially processed zaken may remain in OpenZaak and will be cleaned up on the next run. |
