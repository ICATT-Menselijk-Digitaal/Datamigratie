# Data Mapping and Transformation

During migration from DET to OpenZaak, certain field values must be transformed to comply with OpenZaak's constraints. The following rules are applied automatically during migration:

## Zaak Field Mappings

| DET Field | OpenZaak Field | Transformation | Max Length |
|-----------|----------------|----------------|------------|
| `Omschrijving` | `Omschrijving` | Truncated with "..." suffix if too long | 80 characters |
| `FunctioneleIdentificatie` | `Identificatie` | **Migration fails** if exceeds max length | 40 characters |
| `Startdatum` | `Startdatum` | Formatted as `yyyy-MM-dd` | - |
| `CreatieDatumTijd` | `Registratiedatum` | Formatted as `yyyy-MM-dd` | - |
| `RedenStart` | `Toelichting` | Truncated with "..." suffix if too long | 1000 characters |
| `Betaalgegevens.TransactieDatum` | `LaatsteBetaaldatum` | Formatted as `yyyy-MM-dd` (optional) | - |
| `ArchiveerGegevens.BewaartermijnEinddatum` | `Archiefactiedatum` | Formatted as `yyyy-MM-dd` (optional) | - |
| `ExterneIdentificatie` | `Kenmerken` | Used constant value "e-Suite" for field `Kenmerken[].bron` | - |

## Besluit Field Mappings

| DET Field | OpenZaak Field | Transformation | Max Length |
|-----------|----------------|----------------|------------|
| `FunctioneleIdentificatie` | `Identificatie` | Truncated with "..." suffix if too long | 50 characters |
| `ingangsdatum` | `ingangsdatum` | If not present then the value is set to 0001-01-01

## Document Field Mappings

| DET Field | OpenZaak Field | Transformation | Max Length |
|-----------|----------------|----------------|------------|
| `Titel` | `Titel` | Truncated with "..." suffix if too long | 200 characters |
| `Beschrijving` | `Beschrijving` | Truncated with "..." suffix if too long | 1000 characters |
| `Kenmerk` | `Identificatie` | **Migration fails** if exceeds max length | 40 characters |

## Roltype Mappings

Each DET zaaktype has a fixed set of 8 rollen that must be fully mapped before a migration can start. For each rol, the user configures which OpenZaak roltype URL it maps to, or selects the special value `alleen_pdf` to indicate that the rol information should only appear in the generated PDF and not be created as a rol in OpenZaak.

| DET Rol         | Configurable target            |
| --------------- | ------------------------------ |
| Initiator       | OZ Roltype URL or `alleen_pdf` |
| Behandelaar     | OZ Roltype URL or `alleen_pdf` |
| Belanghebbende  | OZ Roltype URL or `alleen_pdf` |
| Gemachtigde     | OZ Roltype URL or `alleen_pdf` |
| Melder          | OZ Roltype URL or `alleen_pdf` |
| Medeaanvrager   | OZ Roltype URL or `alleen_pdf` |
| Plaatsvervanger | OZ Roltype URL or `alleen_pdf` |
| Overig          | OZ Roltype URL or `alleen_pdf` |

**Validation:** All 8 rollen must be mapped before a migration can be started. A migration will be blocked with an error if any mapping is missing.

## Truncation Behavior

When a field value exceeds its maximum allowed length, the following truncation logic is applied:

1. If the value is within the limit, it is used as-is
2. If the value exceeds the limit, it is truncated and "..." is appended
3. The final length (including "...") equals the maximum allowed length
4. Trailing whitespace is removed before appending "..."

**Example:**
- Input: `"Hello world"`, Max length: `5`
- Output: `"He..."` (length = 5)
