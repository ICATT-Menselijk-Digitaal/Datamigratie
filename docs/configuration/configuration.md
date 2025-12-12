# Configuratie

## Environment Variabelen

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

Let op: De API variabelen gebruiken dubbele underscores (`__`) voor .NET configuratie binding.

## OIDC Configuratie voor UI Inloggen

Om in de UI in te kunnen loggen, moet je in de user secrets van het Datamigratie.Server project de benodigde configuratie invullen:

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
