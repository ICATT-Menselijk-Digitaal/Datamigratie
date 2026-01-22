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
```

### Opstarten

1. Open met Visual Studio
2. Stel Datamigratie.AppHost in als startup project
3. Start de applicatie

## Database migration aanmaken

.NET CLI:

```bash
dotnet ef migrations add MyMigration --project Datamigratie.Data --startup-project Datamigratie.Server
```

Visual Studio Package Manager Console:

```powershell
Add-Migration MyMigration -Project Datamigratie.Data -StartupProject Datamigratie.Server
```