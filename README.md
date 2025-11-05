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

