This document contains findings and results from migrating simulated DET data to OpenZaak using the Datamigratie application.

# Research Topic: OpenZaak mass deletion

**Question:** After running a migration with simulated data, how can we undo all created data in OpenZaak?

The OpenZaak beheer UI supports mass deletion of zaken, documents, and besluiten via name-based search. Zaken, documents, and besluiten must each be deleted separately. To avoid accidentally deleting unrelated data in a shared environment, all simulated zaken, documents, and besluiten are tagged with `- TEST`, making them easy to find via search.

**Process:** Search for `TEST` in the UI, select all results, and delete. This was tested against ~800 zaken with ~1000 documents. All zaken and documents were successfully deleted, though the UI sometimes displayed an error page after the operation.
