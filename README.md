# Datamigratie

De Datamigratie Tool (DMT) stelt gemeenten in staat om zaken te migreren vanuit de E-Suite naar OpenZaak

## Documentatie

De documentatie staat op readthedocs: https://datamigratie.readthedocs.io/

## Lokaal opstarten

### Vereisten

Om de applicatie lokaal te draaien, heb je het volgende nodig:

1. Een DET omgeving met de benodigde API toegang. Vul deze in in via de environment variabelen:

```
DetApi__BaseUrl
DetApi__ApiKey
```

2. Een OpenZaak omgeving met de benodigde API toegang. Vul deze in via de environment variabelen:

```
OpenZaakApi__BaseUrl
OpenZaakApi__ApiUser
OpenZaakApi__ApiKey
```

3. Een OIDC provider voor authenticatie. Vul deze in via de environment variabelen:

```
Oidc__Authority
Oidc__ClientId
Oidc__ClientSecret
Oidc__FunctioneelBeheerderRole

FeatureFlags__EnableTestHelpers
```

### Feature Flags

De applicatie gebruikt feature flags om bepaalde functionaliteit in of uit te schakelen. Deze kunnen geconfigureerd worden via environment variabelen.

#### Beschikbare Feature Flags

| Flag                | Environment Variable              | Standaard | Beschrijving                                                                                                                                                       |
| ------------------- | --------------------------------- | --------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| `EnableTestHelpers` | `FeatureFlags__EnableTestHelpers` | `false`   | Schakelt test helper UI in voor batch selectie bij het mappen van grote aantallen documenttypes. Biedt een pre-selectie optie om het mapping proces te versnellen. |

#### Configuratie

**Via environment variabelen:**

```bash
export FeatureFlags__EnableTestHelpers=true
```

**Via Helm (values.yaml):**

```yaml
featureFlags:
  enableTestHelpers: true
```

Feature flags worden geladen door de frontend via het `/api/app-version` endpoint en zijn beschikbaar via het `featureFlags` object in Vue componenten.

### Opstarten

## Optie 1: Visual Studio + vscode

1. Open met Visual Studio
2. Stel Datamigratie.AppHost in als startup project
3. Start de applicatie
4. Open de root folder van het project (`./Datamigratie`) in vscode
5. Gebruik vscode voor frontend wijzigingen, zodat je code automatisch geformatteerd wordt volgens de regels (prettier + eslint)

## Optie 2: Alleen vscode

1. Open de root folder van het project (`./Datamigratie`) in vscode
2. Start de applicatie met het debug profiel of met `dotnet run`

## Database migration aanmaken

.NET CLI:

```bash
dotnet ef migrations add MyMigration --project Datamigratie.Data --startup-project Datamigratie.Server
```

Visual Studio Package Manager Console:

```powershell
Add-Migration MyMigration -Project Datamigratie.Data -StartupProject Datamigratie.Server
```
