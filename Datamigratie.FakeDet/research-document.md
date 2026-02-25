This document contains findings and results from migrating simulated DET data to OpenZaak using the Datamigratie application.

# Performance Testing: Initial Findings

## Test Setup

- **Total zaken tested:** 76 successful zaken
- **Document load:** 54 zaken containing one ~10 MB document with 1 version
- **Besluiten:** None linked

## Results

- **Average migration time (zaak with one 10 MB document):** ~1.7 seconds
- **Difference between a zaak with vs. without a document:** ~0.70 seconds

## Average Zaak Trace

The trace below is representative of a zaak with one 10 MB document.

| Span                                                                                                | Service              | Duration |
| --------------------------------------------------------------------------------------------------- | -------------------- | -------- |
| MigrateZaak                                                                                         | datamigratie-server  | 1.89s    |
| &nbsp;&nbsp;CreateZaak                                                                              | datamigratie-server  | 0.14s    |
| &nbsp;&nbsp;&nbsp;&nbsp;HTTP POST 201                                                               | datamigratie-server  | 0.11s    |
| &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;POST zaken/.../zaken                                            | openzaak             | 0.10s    |
| &nbsp;&nbsp;&nbsp;&nbsp;HTTP GET 200                                                                | datamigratie-server  | 43.33ms  |
| &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;GET catalogi/.../zaaktype-informatieobjecttypen                 | openzaak             | 38.08ms  |
| &nbsp;&nbsp;MigrateResultaat                                                                        | datamigratie-server  | 0.20s    |
| &nbsp;&nbsp;&nbsp;&nbsp;HTTP POST 201                                                               | datamigratie-server  | 0.16s    |
| &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;POST zaken/.../resultaten                                       | openzaak             | 0.15s    |
| &nbsp;&nbsp;MigrateStatus                                                                           | datamigratie-server  | 0.26s    |
| &nbsp;&nbsp;&nbsp;&nbsp;HTTP POST 201                                                               | datamigratie-server  | 0.23s    |
| &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;POST zaken/.../statussen                                        | openzaak             | 0.22s    |
| &nbsp;&nbsp;UploadZaakgegevensPdf                                                                   | datamigratie-server  | 0.52s    |
| &nbsp;&nbsp;&nbsp;&nbsp;GenerateZaakgegevensPdf                                                     | datamigratie-server  | 8.21ms   |
| &nbsp;&nbsp;&nbsp;&nbsp;HTTP POST 201                                                               | datamigratie-server  | 0.10s    |
| &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;POST documenten/.../enkelvoudiginformatieobjecten               | openzaak             | 93.50ms  |
| &nbsp;&nbsp;&nbsp;&nbsp;HTTP PUT 200                                                                | datamigratie-server  | 55.72ms  |
| &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;PUT documenten/.../bestandsdelen/{uuid}                         | openzaak             | 49.06ms  |
| &nbsp;&nbsp;&nbsp;&nbsp;HTTP POST 204                                                               | datamigratie-server  | 67.40ms  |
| &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;POST documenten/.../enkelvoudiginformatieobjecten/{uuid}/unlock | openzaak             | 60.89ms  |
| &nbsp;&nbsp;&nbsp;&nbsp;HTTP POST 201                                                               | datamigratie-server  | 0.17s    |
| &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;POST zaken/.../zaakinformatieobjecten                           | openzaak             | 0.16s    |
| &nbsp;&nbsp;MigrateDocuments                                                                        | datamigratie-server  | 0.69s    |
| &nbsp;&nbsp;&nbsp;&nbsp;HTTP POST 201                                                               | datamigratie-server  | 0.15s    |
| &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;POST documenten/.../enkelvoudiginformatieobjecten               | openzaak             | 0.14s    |
| &nbsp;&nbsp;&nbsp;&nbsp;HTTP GET 200                                                                | datamigratie-server  | 31.59ms  |
| &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;GET /documenten/inhoud/{id}                                     | datamigratie-fakedet | 91.83ms  |
| &nbsp;&nbsp;&nbsp;&nbsp;HTTP PUT 200                                                                | datamigratie-server  | 98.38ms  |
| &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;PUT documenten/.../bestandsdelen/{uuid}                         | openzaak             | 44.64ms  |
| &nbsp;&nbsp;&nbsp;&nbsp;HTTP POST 204                                                               | datamigratie-server  | 0.13s    |
| &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;POST documenten/.../enkelvoudiginformatieobjecten/{uuid}/unlock | openzaak             | 0.12s    |
| &nbsp;&nbsp;&nbsp;&nbsp;HTTP POST 201                                                               | datamigratie-server  | 0.16s    |
| &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;POST zaken/.../zaakinformatieobjecten                           | openzaak             | 0.15s    |
| &nbsp;&nbsp;MigrateBesluiten                                                                        | datamigratie-server  | —        |

**Trace metadata:** Duration 1.89s · Resources 5 · Depth 4 · Total spans 34

## Observations

- **UploadZaakgegevensPdf** (the summary PDF attached to every zaak) accounts for ~0.52s of the total trace.
  - PDF generation itself is very fast: ~8ms.
  - Linking the PDF to the zaak (zaakinformatieobjecttype koppeling) takes longer (0.17s) than uploading the file content (0.11s). The generated file is about 16 KB.
- **Documents are the heaviest part of the migration.** Fetching and posting a document take comparable time (~0.1–0.15s each for network calls).
- With fake data, documents are generated and served as PDFs by FakeDet. It is unclear how performance will differ when fetching real documents from the eSuite, as the fetch mechanism and document sizes may differ significantly.

## Open Questions

- How does migration perform when besluiten are included?
- How does performance scale with multiple documents and/or multiple document versions per zaak?
- Are documents faster to process when fetched directly from an API (real eSuite data) rather than generated on-the-fly by FakeDet?

---

# Errors Encountered During Performance Testing

## Validatiefout: `identificatie-niet-uniek`

**Description**
All migrated zaken are migrated again at the end of the migration, resulting in this error.

**Error:**

```
{ Name = identificatie, Reason = Deze identificatie (GEMTE821002193-20904-000100-TEST) bestaat al voor deze bronorganisatie, Code = identificatie-niet-uniek }
```

**Cause:** unknown

---

# Error: Document `identificatie` Too Long for Zaakgegevens PDF

**Description:** When migrating a zaak whose `FunctioneleIdentificatie` is close to or at the 40-character OpenZaak limit, creating the zaakgegevens PDF document fails because the document `Identificatie` is set directly to the zaak identificatie, which may already be at the maximum allowed length.

**Relevant code:** `MigrateZaakService.cs`, `UploadZaakgegevensPdfAsync` — the `Identificatie` field of the generated PDF document is set to `detZaak.FunctioneleIdentificatie` without length validation or truncation, while OpenZaak enforces a maximum of 40 characters on document identificatie.

**Cause:** The zaak identificatie itself is allowed up to 40 characters (`CreateOzZaakCreationRequest` enforces this). A zaak at exactly 40 characters will cause the document creation to fail when that same value is used as the document `Identificatie`.

**Resolution:** Either truncate the identificatie for the PDF document, or derive a shorter fixed identifier (e.g. using the zaak UUID instead of the functional identificatie).

---

# Research Topic: Testing on the Dev Environment

The dev environment (already deployed) is more representative of the real production scenario than local testing, since it uses real infrastructure and a real network connection to OpenZaak. However, it currently has no observability — Aspire is not deployed, so there are no traces or metrics.

**To enable this:** The app already has OpenTelemetry configured via `ServiceDefaults`. Adding Azure Monitor (Application Insights) requires only setting `APPLICATIONINSIGHTS_CONNECTION_STRING` on the deployed services, with a small addition to `ServiceDefaults`. This would give the same trace and log data as the local Aspire dashboard, allowing direct comparison between local and dev performance results.

---

# Research Topic: OpenZaak mass deletion

**Question:** After running a migration with simulated data, how can we undo all created data in OpenZaak?

The OpenZaak beheer UI supports mass deletion of zaken, documents, and besluiten via name-based search. Zaken, documents, and besluiten must each be deleted separately. To avoid accidentally deleting unrelated data in a shared environment, all simulated zaken, documents, and besluiten are tagged with `- TEST`, making them easy to find via search.

**Process:** Search for `TEST` in the UI, select all results, and delete. This was tested against ~800 zaken with ~1000 documents. All zaken and documents were successfully deleted, though the UI sometimes displayed an error page after the operation.
