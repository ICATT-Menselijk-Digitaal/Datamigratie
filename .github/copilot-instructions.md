# GitHub Copilot PR Review Instructions

This document provides guidelines for reviewing pull requests in the Datamigratie repository. Use these patterns and conventions when generating PR review feedback.

## Architecture Overview

**Datamigratie Tool (DMT)** is a Dutch government data migration application for migrating case management data from E-Suite (DET) to OpenZaak.

### Tech Stack
- **Backend**: .NET 8, ASP.NET Core, Entity Framework Core 8, PostgreSQL
- **Frontend**: Vue 3 (Composition API), TypeScript, Vite
- **Orchestration**: .NET Aspire
- **External APIs**: DET (Data Extraction Tool), OpenZaak

### Project Structure
| Project | Purpose |
|---------|---------|
| `Datamigratie.AppHost` | Aspire orchestration entry point |
| `Datamigratie.Server` | REST API + Vue SPA hosting |
| `Datamigratie.MigrationService` | Background worker for DB migrations |
| `Datamigratie.Common` | Shared API clients (DET, OpenZaak), models |
| `Datamigratie.Data` | EF Core DbContext, entities, migrations |
| `datamigratie.client` | Vue 3 TypeScript frontend |

### Architecture Pattern
**Vertical Slice / Feature-Driven Design**: Each feature is self-contained in `/Features/[Feature]/[SubFeature]/` with its own controller, service, and models.

---

## Code Conventions

### Backend (C#)

**File Organization:**
```
Features/
  └── [Feature]/
      └── [SubFeature]/
          ├── [Action]Controller.cs
          ├── [Action]Service.cs
          └── Models/
```

**Patterns:**
- Primary constructor injection: `public class Service(IDependency dep) : IService`
- Interface-first for services: `IMapZaaktypenService` + `MapZaaktypenService` (in the same file)
- Async/await throughout all I/O operations
- EF Core with snake_case naming (via EFCore.NamingConventions)

**Naming:**
- PascalCase for classes, interfaces, methods, properties
- Interface prefix: `I` (e.g., `IDetApiClient`)
- Request models: `{Action}{Entity}Request`
- Response models: `{Entity}Response`

### Frontend (Vue/TypeScript)

**Component Structure:**
```vue
<template>
  <!-- Semantic HTML with accessibility -->
</template>

<script setup lang="ts">
// Imports, props, emits, state, computed, methods, lifecycle
</script>

<style lang="scss" scoped>
/* CSS custom properties for theming */
</style>
```

**Patterns:**
- Composition API with `<script setup>`
- Composables for shared state: `use{Feature}.ts` (not Vuex/Pinia)
- Service layer for API calls: `{entity}Service.ts`
- Type definitions co-located with services

**Formatting:**
- ESLint + Prettier enforced
- 2-space indentation, 100 char width
- No trailing commas, semicolons required

---

## PR Review Checklist

### Architecture & Design

- [ ] **Feature isolation**: Code belongs in feature folder, not spread across shared locations unless truly reusable
  > *"This is not something we will share with other features, so from a feature slice architecture point of view, this should be in a folder under the feature"* (PR #45)

- [ ] **Avoid premature abstraction**: Only add interfaces if mocking or multiple implementations are needed
  > *"Wat is je overweging om deze abstractie toe te passen? Ben je van plan een tweede implementatie of een mock te maken?"* (PR #15)

- [ ] **Validate acceptance criteria**: Ensure all fields/requirements from the story are implemented
  > *"This doesn't contain all the fields from the story"* (PR #45)

### Code Quality

- [ ] **Avoid duplication**: Abstract repeated patterns into shared functions
  > *"The whole process of all these actions are pretty much duplicated. Have you considered some abstraction?"* (PR #45)

- [ ] **Meaningful defaults**: Don't use zero/dummy values when "not computed" differs from "zero" - use nullable
  > *"Logically 'there are zero records' is not equal to 'it's not computed yet', so it should be nullable"* (PR #70)

- [ ] **Use GUIDs over URLs**: For identifiers, extract GUIDs rather than storing full URLs (environment safety)
  > *"It's risky to use the complete returned URL. Sometimes different domains are used internally and externally. Better to extract the GUID"* (PR #50)

- [ ] **Explicit null handling**: Distinguish "not found" (return null) from errors (throw exception)
  > *"Effectief kan dit niet null zijn? Als het zaaktype niet gevonden kan worden, krijgen we een 404 en dus een exceptie?"* (PR #15)

### Error Handling

- [ ] **Proper status codes**: Use 404 for not found, 400 for validation errors, 409 for conflicts
  > *"Explicietere (custom) excepties zodat je specifieke catch statements kan doen en weet of een specifieke foutmelding nuttig is"* (PR #15)

- [ ] **API client pattern**: Return null on 404, then call `EnsureSuccessStatusCode()` for other errors
  > *"Bij een 404 een null returnen, daarna EnsureSuccessStatusCode aanroepen, en daarna pas het object teruggeven"* (PR #15)

- [ ] **Don't expose raw exceptions**: Be intentional about what error messages reach the API response
  > *"Verwacht je dat de message van de exceptie altijd iets is wat je in de api response wil zien?"* (PR #15)

- [ ] **Defensive deserialization**: Handle unexpected null/empty responses from external APIs
  > *"Als dit in de praktijk voorkomt, dan duidt het waarschijnlijk op een onbekende bug in de API. We maken analyseren makkelijker als we het nu afvangen"* (PR #35)

### Frontend Consistency

- [ ] **Reuse helper functions**: Share utilities like date formatting across views
  > *"In ZaaktypeView.vue zit een formatDateTime functie. Lijkt me goed om door de hele frontend dezelfde formattering te houden"* (PR #65)

- [ ] **Native form validation**: Use `setCustomValidity()` instead of custom validation state
  > *"Voorstel: alleen de 11-proef validatie doen en inprikken op InputElement.setCustomValidity. Dan doet de validatie mee in de native HTML form validatie"* (PR #65)

- [ ] **Consistent styling**: Use CSS custom properties, don't create per-page custom styles
  > *"Aandacht voor ontwerp betekent dat je de hele frontend meeneemt en niet per pagina het wiel opnieuw uitvindt. Gebruik css variables voor kleur en spacing"* (PR #65)

- [ ] **Consistent behavior**: Error/success messages should work the same way across pages
  > *"Niet alleen styling, maar ook gedrag (error/succes meldingen) wijkt af. Consistentie is belangrijk"* (PR #65)

### Data Types & Models

- [ ] **Use proper types**: `Guid` instead of `string` for UUIDs
  > *"Als de UUID toegevoegde waarde heeft, laten we dan een Guid gebruiken ipv een string"* (PR #15)

- [ ] **Complete model mapping**: Ensure all required API fields are mapped correctly
  > *"with source model you mean our model? if it's missing there you can just add it"* (PR #45)

### Database & Migrations

- [ ] **Migration impact**: Editing existing migrations requires all developers to recreate their database
  > *"Een bestaande migratie aanpassen betekent dat alle ontwikkelaars de bestaande database moeten weggooien"* (PR #15)

- [ ] **Constraint design**: Consider relationship cardinality (one-to-one vs one-to-many) with stakeholders
  > *"Zou er niet ook een unique constraint op OzZaaktypeId moeten?"* (PR #15)

### Dependencies & Configuration

- [ ] **Version alignment**: Keep framework versions consistent (SDK matching runtime)
  > *"Klopt dit? Correspondeert dit niet met de versie van dotnet (8)?"* (PR #10)

- [ ] **Stable port configuration**: Consider fixed ports for local development convenience
  > *"Irritant als de DB port telkens weer iets randoms wordt"* (PR #15)

---

## Common Review Comments (Quick Reference)

| Issue | Feedback Pattern |
|-------|-----------------|
| Code in wrong location | "Should be in a folder under the feature, not shared" |
| Unnecessary abstraction | "Do you plan to create a second implementation or mock?" |
| Repeated code | "Have you considered some abstraction?" |
| Zero instead of null | "Zero is not equal to 'not computed yet'" |
| Full URL as identifier | "Extract the GUID, different domains may be used" |
| Custom validation state | "Use setCustomValidity for native HTML validation" |
| Per-page custom styling | "Use CSS variables, keep pages consistent" |
| Raw exception in response | "Be explicit about what reaches the API response" |
| Missing story fields | "This doesn't contain all fields from the story" |

---

## Language Notes

- Code comments and technical discussions: English preferred
- UI strings and user-facing content: Dutch (Netherlands)
- PR discussions: Mix of Dutch and English is acceptable
