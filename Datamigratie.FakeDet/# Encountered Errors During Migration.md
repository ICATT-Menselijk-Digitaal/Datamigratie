# Encountered Errors During Migration

## Identificatie cannot be empty

Error:

Migratie onderbroken: versie 1 van document 'Kennisgeving - commodi in-TEST' (bestand: kennisgeving_ffxwc6gb.pdf) kon niet worden gemigreerd | HTTP 400: Validatie fouten: { Name = identificatie, Reason = Dit veld mag niet leeg zijn., Code = null }

Solved by:
The kenmerk of a DET document is used as an ID for a OZ document. This kenmerk can be null and OZ will return this error. Solved by making Kenmerk whitespace ("") when not present (dicussed with PO)

## identificatie already exists

Error:

Validatie fouten: { Name = identificatie, Reason = Deze identificatie (GEMTE821002193-20904-000001-TEST) bestaat al voor deze bronorganisatie, Code = identificatie-niet-uniek }

Solved by:
Unknown, duplicated data generation? Error exists when starting from a clean environment

# Migration of Documents

Currently we generate documents with a size of a limited random value, this can be found in the variable: Documentgrootte
