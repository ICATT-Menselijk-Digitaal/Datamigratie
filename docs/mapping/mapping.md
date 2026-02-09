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

## Document Field Mappings

| DET Field | OpenZaak Field | Transformation | Max Length |
|-----------|----------------|----------------|------------|
| `Titel` | `Titel` | Truncated with "..." suffix if too long | 200 characters |
| `Beschrijving` | `Beschrijving` | Truncated with "..." suffix if too long | 1000 characters |
| `Kenmerk` | `Identificatie` | **Migration fails** if exceeds max length | 40 characters |

## Truncation Behavior

When a field value exceeds its maximum allowed length, the following truncation logic is applied:

1. If the value is within the limit, it is used as-is
2. If the value exceeds the limit, it is truncated and "..." is appended
3. The final length (including "...") equals the maximum allowed length
4. Trailing whitespace is removed before appending "..."

**Example:**
- Input: `"Hello world"`, Max length: `5`
- Output: `"He..."` (length = 5)
