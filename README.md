# Datamigratie

## Run locally
1. Open with Visual Studio 2022 
2. Set Datamigratie.AppHost as startup project
3. Run the application

## Data Extraction Tool (DET)
[Open API spec](https://redocly.github.io/redoc/?url=https://esuite-data-extractie-gcp2.esuite-development.net/q/openapi)


## Installation

### Environment Variabelen
De applicatie gebruikt de volgende environment variabelen:

```
OpenZaakApi__BaseUrl
OpenZaakApi__ApiUser
OpenZaakApi__ApiKey

DetApi__BaseUrl
DetApi__ApiKey

ConnectionStrings__Datamigratie

Oidc__Authority
Oidc__ClientId
Oidc__ClientSecret
Oidc__FunctioneelBeheerderRole
```

### Inloggen in de UI
- Om in de UI in te kunnen loggen, moet je in de user secrets van het Datamigratie.Server project de benodigde configuratie invullen:
```json
{
  "Oidc": {
    "Authority": "",
    "ClientId": "",
    "ClientSecret": "",
    "FunctioneelBeheerderRole": ""
  }
}
```

De API variabelen gebruiken dubbele underscores (`__`) voor .NET configuratie binding.

### Helm Deployment
1. Kopieer `charts/datamigratie/values.yaml` naar eigen values file
2. Pas de waardes aan voor jouw omgeving (API keys, database, etc.)
3. Installeer met Helm:

```
helm install datamigratie ./charts/datamigratie -f jouw-values.yaml
```

## Data Mapping and Transformation Rules

During migration from DET to OpenZaak, certain field values must be transformed to comply with OpenZaak's constraints. The following rules are applied automatically by the `MigrateZaakService`:

### Zaak Field Mappings

| DET Field | OpenZaak Field | Transformation | Max Length |
|-----------|----------------|----------------|------------|
| `Omschrijving` | `Omschrijving` | Truncated with "..." suffix if too long | 80 characters |
| `FunctioneleIdentificatie` | `Identificatie` | No transformation | - |
| `Startdatum` | `Startdatum` | Formatted as `yyyy-MM-dd` | - |
| `CreatieDatumTijd` | `Registratiedatum` | Formatted as `yyyy-MM-dd` | - |

### Document Field Mappings

| DET Field | OpenZaak Field | Transformation | Max Length |
|-----------|----------------|----------------|------------|
| `Titel` | `Titel` | Truncated with "..." suffix if too long | 200 characters |
| `Beschrijving` | `Beschrijving` | Truncated with "..." suffix if too long | 1000 characters |
| `Kenmerk` | `Identificatie` | **Migration fails** if exceeds max length | 40 characters |

### Truncation Behavior

When a field value exceeds its maximum allowed length, the following truncation logic is applied:

1. If the value is within the limit, it is used as-is
2. If the value exceeds the limit, it is truncated and "..." is appended
3. The final length (including "...") equals the maximum allowed length
4. Trailing whitespace is removed before appending "..."

**Example:**
- Input: `"Hello world"`, Max length: `5`
- Output: `"He..."` (length = 5)

### Migration Failures

The migration will **fail** for a document if:
- The `Kenmerk` field exceeds 40 characters (this field cannot be truncated as it serves as a unique identifier)

When this occurs, an error message is returned indicating which document caused the failure and why.

### Implementation Details

All transformation logic is implemented in [MigrateZaakService.cs](Datamigratie.Server/Features/MigrateZaak/MigrateZaakService.cs):
- `TruncateWithDots()` method handles string truncation (line 161)
- `MapToOzDocument()` method applies document field transformations (line 75)
- `CreateOzZaakCreationRequest()` method applies zaak field transformations (line 112)

