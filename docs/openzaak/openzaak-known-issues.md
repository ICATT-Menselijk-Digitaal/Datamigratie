# Bug Report — `AttributeError: 'NoneType' object has no attribute 'uuid'`

## Description

When fetching `zaakinformatieobjecten` for a specific zaak, an HTTP 500 occurs with the following error:

```
AttributeError at /zaken/api/v1/zaakinformatieobjecten
'NoneType' object has no attribute 'uuid'

Request Method: GET
Request URL:    https://openzaak.dev.kiss-demo.nl/zaken/api/v1/zaakinformatieobjecten
                ?zaak=https%3A%2F%2Fopenzaak.dev.kiss-demo.nl%2Fzaken%2Fapi%2Fv1%2Fzaken%2F5ed292f7-18f5-4700-8939-c24f1c67995d
```

## Root Cause

The error is triggered when a `ZaakInformatieobject` link exists in OpenZaak that points to a document that has since been **deleted**. When the document is deleted, the link is not automatically removed. OpenZaak then tries to resolve the UUID of the linked document — but the document no longer exists, causing the server to crash with `AttributeError: 'NoneType' object has no attribute 'uuid'`.

This is **not** related to a partially created document — the document itself was fully created and linked, but deleted afterwards (e.g. via the admin portal or API).

## Steps to Reproduce

1. Run a migration for a zaak — a PDF document (`zaakgegevens-<ZAAKNUMMER>`) is created and linked to the zaak.
2. Delete the document via the admin portal or the `DELETE /documenten/api/v1/enkelvoudiginformatieobjecten/{uuid}` endpoint.
3. In the zaak detail page, the `ZaakInformatieobject` section still shows the entry, but now with the identification `<ZAAKNUMMER> - None` — the document reference is broken.
4. Any subsequent call to `GET /zaken/api/v1/zaakinformatieobjecten?zaak=...` for this zaak returns HTTP 500:

```
AttributeError at /zaken/api/v1/zaakinformatieobjecten
'NoneType' object has no attribute 'uuid'
```

## Workaround

Attempting to delete the dangling `ZaakInformatieobject` link via the admin portal also fails with the same error:

```
AttributeError at /admin/zaken/zaak/1746/change/
'NoneType' object has no attribute 'uuid'

Request Method: POST
Request URL:    https://openzaak.dev.kiss-demo.nl/admin/zaken/zaak/1746/change/
                ?_changelist_filters=q%3D175-2023
```

The only working workaround is to **delete the entire zaak via the admin portal** and trigger a full re-migration. Once the zaak is deleted (including all its linked objects), the migration tool can recreate it cleanly.

## Impact

- Any zaak with a dangling `ZaakInformatieobject` link (document deleted, link not cleaned up) becomes unmigrateable.
- The HTTP 500 prevents our migration tool from reading existing links, which blocks the entire cleanup-and-recreate flow.
- Manual intervention is required per affected zaak.

## Known Issue

This bug is tracked in the OpenZaak repository as [open-zaak/open-zaak#2347 — 500 error when requesting ZIO without a link to an informatieobject](https://github.com/open-zaak/open-zaak/issues/2347).

**Status:** Open — a fix is in progress via [PR #2363 — 🐛 [#2347] override eio admin delete & fix zio update](https://github.com/open-zaak/open-zaak/pull/2363), assigned to [@Floris272](https://github.com/Floris272). The issue is labelled **approved** and was moved to _In Progress_ in March 2026.

**Root cause confirmed by OpenZaak team:** Deleting an `EnkelvoudigInformatieobject` via the admin interface is allowed even when a `ZaakInformatieobject` link exists pointing to it. After deletion, the `informatieobject` field on the ZIO becomes `None`, which causes a crash when the ZIO is queried or when its delete page is accessed via the admin portal.

## Questions

1. When is PR #2363 expected to be released, and in which version of OpenZaak will the fix be included?
2. Until the fix is released, is there a supported SQL query or admin procedure to bulk-identify and remove all dangling `ZaakInformatieobject` records (i.e. those where `informatieobject` is `NULL`)?
3. Will the fix include a data migration to clean up existing dangling records, or only prevent new ones from being created?
