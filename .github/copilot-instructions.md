# Project guidelines

## Architecture Overview

**Datamigratie Tool (DMT)** is a Dutch government data migration application for migrating case management data from E-Suite (DET) to OpenZaak.

### Tech Stack

- **Backend**: .NET 8, ASP.NET Core, Entity Framework Core 8, PostgreSQL
- **Frontend**: Vue 3 (Composition API), TypeScript, Vite
- **Orchestration**: .NET Aspire
- **External APIs**: DET (Data Extraction Tool), OpenZaak

### Project Structure

| Project                         | Purpose                                    |
| ------------------------------- | ------------------------------------------ |
| `Datamigratie.AppHost`          | Aspire orchestration entry point           |
| `Datamigratie.Server`           | REST API + Vue SPA hosting                 |
| `Datamigratie.MigrationService` | Background worker for DB migrations        |
| `Datamigratie.Common`           | Shared API clients (DET, OpenZaak), models |
| `Datamigratie.Data`             | EF Core DbContext, entities, migrations    |
| `datamigratie.client`           | Vue 3 TypeScript frontend                  |

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

**Best practices**
Before writing or modifying any frontend code, read the Frontend / CSS Best Practices section below. Key principles: use semantic HTML over divs, prefer browser defaults over custom CSS/JS, and use the project's CSS custom properties — never hardcode spacing, color, or typography values.

---

## PR Review Checklist

### Architecture & Design

- [ ] **Feature isolation**: Code belongs in feature folder, not spread across shared locations unless truly reusable

  > _"This is not something we will share with other features, so from a feature slice architecture point of view, this should be in a folder under the feature"_ (PR #45)

- [ ] **Avoid premature abstraction**: Only add interfaces if mocking or multiple implementations are needed

  > _"Wat is je overweging om deze abstractie toe te passen? Ben je van plan een tweede implementatie of een mock te maken?"_ (PR #15)

- [ ] **Validate acceptance criteria**: Ensure all fields/requirements from the story are implemented
  > _"This doesn't contain all the fields from the story"_ (PR #45)

### Code Quality

- [ ] **Avoid duplication**: Abstract repeated patterns into shared functions

  > _"The whole process of all these actions are pretty much duplicated. Have you considered some abstraction?"_ (PR #45)

- [ ] **Meaningful defaults**: Don't use zero/dummy values when "not computed" differs from "zero" - use nullable

  > _"Logically 'there are zero records' is not equal to 'it's not computed yet', so it should be nullable"_ (PR #70)

- [ ] **Use GUIDs over URLs**: For identifiers, extract GUIDs rather than storing full URLs (environment safety)

  > _"It's risky to use the complete returned URL. Sometimes different domains are used internally and externally. Better to extract the GUID"_ (PR #50)

- [ ] **Explicit null handling**: Distinguish "not found" (return null) from errors (throw exception)
  > _"Effectief kan dit niet null zijn? Als het zaaktype niet gevonden kan worden, krijgen we een 404 en dus een exceptie?"_ (PR #15)

### Error Handling

- [ ] **Proper status codes**: Use 404 for not found, 400 for validation errors, 409 for conflicts

  > _"Explicietere (custom) excepties zodat je specifieke catch statements kan doen en weet of een specifieke foutmelding nuttig is"_ (PR #15)

- [ ] **API client pattern**: Return null on 404, then call `EnsureSuccessStatusCode()` for other errors

  > _"Bij een 404 een null returnen, daarna EnsureSuccessStatusCode aanroepen, en daarna pas het object teruggeven"_ (PR #15)

- [ ] **Don't expose raw exceptions**: Be intentional about what error messages reach the API response

  > _"Verwacht je dat de message van de exceptie altijd iets is wat je in de api response wil zien?"_ (PR #15)

- [ ] **Defensive deserialization**: Handle unexpected null/empty responses from external APIs
  > _"Als dit in de praktijk voorkomt, dan duidt het waarschijnlijk op een onbekende bug in de API. We maken analyseren makkelijker als we het nu afvangen"_ (PR #35)

### Frontend Consistency

- [ ] **Reuse helper functions**: Share utilities like date formatting across views

  > _"In ZaaktypeView.vue zit een formatDateTime functie. Lijkt me goed om door de hele frontend dezelfde formattering te houden"_ (PR #65)

- [ ] **Native form validation**: Use `setCustomValidity()` instead of custom validation state

  > _"Voorstel: alleen de 11-proef validatie doen en inprikken op InputElement.setCustomValidity. Dan doet de validatie mee in de native HTML form validatie"_ (PR #65)

- [ ] **Consistent styling**: Use CSS custom properties, don't create per-page custom styles

  > _"Aandacht voor ontwerp betekent dat je de hele frontend meeneemt en niet per pagina het wiel opnieuw uitvindt. Gebruik css variables voor kleur en spacing"_ (PR #65)

- [ ] **Consistent behavior**: Error/success messages should work the same way across pages
  > _"Niet alleen styling, maar ook gedrag (error/succes meldingen) wijkt af. Consistentie is belangrijk"_ (PR #65)

### Data Types & Models

- [ ] **Use proper types**: `Guid` instead of `string` for UUIDs

  > _"Als de UUID toegevoegde waarde heeft, laten we dan een Guid gebruiken ipv een string"_ (PR #15)

- [ ] **Complete model mapping**: Ensure all required API fields are mapped correctly
  > _"with source model you mean our model? if it's missing there you can just add it"_ (PR #45)

### Database & Migrations

- [ ] **Convention over configuration**: Trust EF Core conventions instead of explicit configuration when the convention produces the correct result. Don't add `.ToTable()`, `.HasColumnName()`, or other fluent API calls that just repeat what EF would infer automatically. Less code = less noise = fewer bugs.

  > _"boyscout rule: laat het netter achter dan hoe je het aantrof. minder code = minder ruis = minder fouten."_ (PR #86)

- [ ] **Migration impact**: Editing existing migrations requires all developers to recreate their database

  > _"Een bestaande migratie aanpassen betekent dat alle ontwikkelaars de bestaande database moeten weggooien"_ (PR #15)

- [ ] **Constraint design**: Consider relationship cardinality (one-to-one vs one-to-many) with stakeholders
  > _"Zou er niet ook een unique constraint op OzZaaktypeId moeten?"_ (PR #15)

### Dependencies & Configuration

- [ ] **Version alignment**: Keep framework versions consistent (SDK matching runtime)

  > _"Klopt dit? Correspondeert dit niet met de versie van dotnet (8)?"_ (PR #10)

- [ ] **Stable port configuration**: Consider fixed ports for local development convenience
  > _"Irritant als de DB port telkens weer iets randoms wordt"_ (PR #15)

### Administration

- [ ] **Release notes**: The changelog should contain an entry for the story related to this pull request. The entry should be in the 'Current version' section of the changelog. The entry should consist of the title of the story and a link to the story in Jira. Usually the link has this format: 'https://dimpact.atlassian.net/browse/[DATA-XXX]'. The right value for [DATA-XXX] can be found in the name of the current branch.

---

### Testing

- [ ] **Test via public interfaces only**: Do not unit test private methods. Test behavior by observing public outputs (return values, DB state, exceptions) given controlled inputs.
  > Private method tests make code hard to refactor and don't guarantee the full flow works after major edits.

---

## Common Review Comments (Quick Reference)

| Issue                     | Feedback Pattern                                         |
| ------------------------- | -------------------------------------------------------- |
| Code in wrong location    | "Should be in a folder under the feature, not shared"    |
| Unnecessary abstraction   | "Do you plan to create a second implementation or mock?" |
| Repeated code             | "Have you considered some abstraction?"                  |
| Zero instead of null      | "Zero is not equal to 'not computed yet'"                |
| Full URL as identifier    | "Extract the GUID, different domains may be used"        |
| Custom validation state   | "Use setCustomValidity for native HTML validation"       |
| Per-page custom styling   | "Use CSS variables, keep pages consistent"               |
| Raw exception in response | "Be explicit about what reaches the API response"        |
| Missing story fields      | "This doesn't contain all fields from the story"         |

---

## Language Notes

- Code comments and technical discussions: English preferred
- UI strings and user-facing content: Dutch (Netherlands)
- PR discussions: Mix of Dutch and English is acceptable

---

# Frontend / CSS Best Practices

---

## 1. Let the browser do the work — less is more

The rule of thumb is simple:

1. **If you can do it with HTML, use HTML.**
2. **If you can't do it with HTML, use CSS.**
3. **If you can't do it with HTML or CSS, use JavaScript.**

Each layer you add is harder to maintain, harder to debug, and more likely to break. HTML is parsed, accessible, and responsive by default. CSS builds on that. JavaScript should be a last resort for behavior that the platform genuinely can't provide.

An unstyled HTML page is already responsive. Text wraps, content stacks, inputs stretch to fit. Every line of CSS we add is a line that can break, conflict, or need maintenance. Start from the assumption that the browser's defaults are good enough, and only override when you have a specific reason.

In practice, this means:

- **Use `<form>` with `<button type="submit">`** and a submit handler on the form instead of wiring up click handlers on buttons. You get Enter-to-submit, built-in validation, and a clear action hierarchy for free.
- **Use `<details>` / `<summary>`** instead of building collapsible sections with JavaScript and v-if/v-else branches. One native element replaces a `collapsible` prop, toggle state, conditional rendering, and an arrow icon that needs manual rotation.
- **Use `<dl>`** for key-value data instead of tables or label/div combos. It's the right tool and it needs almost no styling.
- **Don't fight natural sizing.** Inputs, selects, and text already know how to size themselves. Avoid setting explicit widths, min-widths, and heights unless content actually overflows.
- **Don't micro-manage spacing.** Set a rhythm at the base level with sensible margins on headings, paragraphs, and form elements. If you're adding `margin-top: 16px` or `gap: 8px` in every component, the base styles are incomplete.

**We don't need to be pixel-perfect with the Figma design.** The goal is to capture the design's _intent_ — its hierarchy, rhythm, and relationships — in a way that works across screen sizes without constant adjustment. A design that needs 50 lines of CSS to position two buttons is telling you the HTML structure is wrong, not that you need more CSS.

---

## 2. Accessibility comes from HTML, not ARIA

If you follow principle 1, most accessibility is already handled. A `<button>` is focusable and keyboard-accessible. A `<form>` announces its structure to screen readers. A `<details>` expands and collapses without JavaScript. A `<div @click>` does none of these things.

The rule is: **don't use ARIA attributes to recreate what a semantic element already provides.** Adding `role="button"` and `tabindex="0"` and a keydown handler to a `<div>` is always worse than just using `<button>`. ARIA is a last resort for cases where no native element exists for the pattern you need.

Beyond element choice, keep these in mind:

- **Every interactive element must be keyboard-accessible.** If you can click it, you should be able to Tab to it and activate it with Enter or Space.
- **Every form input needs a label.** Use `<label>` with a `for` attribute, or wrap the input inside the label. Placeholder text is not a label.
- **Don't rely on color alone** to communicate state. Pair color with text, icons, or other visual indicators. Our `--danger` and `--success` tokens should always be accompanied by a label or icon.
- **Use `aria-labelledby` or `aria-label`** only when a visible label isn't possible — for example, an icon-only button should have `aria-label="Close"`.

---

## 3. Use the CSS custom properties that are already defined

We have a complete set of design tokens in `base.css` and `main.scss`. Use them. Don't introduce magic numbers or one-off values when a variable already exists.

**Spacing** — use these instead of `8px`, `1rem`, `16px` etc.:
`--spacing-extrasmall` (0.25rem), `--spacing-small` (0.5rem), `--spacing-default` (1rem), `--spacing-large` (2rem), `--spacing-extralarge` (6rem)

**Typography:**
`--font-small` (0.875rem), `--font-medium` (1rem), `--font-large` (1.25rem), `--font-extralarge` (2rem), `--font-bold` (600), `--line-height-default` (1.25rem)

**Colors:**
`--bg`, `--accent-bg`, `--text`, `--text-light`, `--border`, `--accent`, `--accent-hover`, `--accent-text`, `--danger`, `--danger-hover`, `--success`, `--disabled`

**Layout:**
`--container-width`, `--container-padding`, `--section-width`, `--section-width-small`, `--section-width-large`, `--input-padding`

**Decorative:**
`--standard-border-radius`, `--radius-default`, `--radius-large`, `--shadow-default`, `--outline-color`, `--outline-width`, `--outline-offset`

If you find yourself writing a literal value like `padding: 0.5rem` or `color: #212121`, check whether a variable covers it. If the right variable doesn't exist yet, consider adding one to the shared definitions rather than hardcoding the value in a component.

This is especially important for colors. Our color palette is defined with a light theme on `:root` and a dark theme via `@media (prefers-color-scheme: dark)`, so theming works automatically — as long as you use the variables. Writing `color: #212121` or `background: white` will break in dark mode. If you need a new color, add it to both theme blocks in `base.css`.

**Tooling:** Install the [CSS Variable Autocomplete](https://marketplace.visualstudio.com/items?itemName=vunguyentuan.vscode-css-variables) extension for VS Code. It gives you autocomplete for all project-defined custom properties, so you don't have to remember what's available — just type `--` and pick from the list.

---

## 4. Normalize typography — don't override it everywhere

Set `font-size`, `font-family`, `font-weight`, and `line-height` once at the base level. Component styles should almost never redeclare these properties. If you find yourself writing `color: var(--text); font-family: var(--sans-font); font-size: var(--font-medium); font-weight: 400; line-height: 1.25;` in scoped styles, that's a sign the base styles are missing something.

---

## 5. Embrace CSS inheritance — don't repeat what's already set

CSS inheritance exists so you don't have to. Properties like `font-family`, `font-size`, `font-weight`, `line-height`, `color`, and `letter-spacing` inherit from parent to child by default. If the base styles set `font-family` on `body` and `color` on `:root`, every element inside already has those values. Redeclaring them is not just redundant — it actively works against you, because now you have multiple places to update when something changes.

Only set a property on a child element if you want it to **differ** from the parent. If you're writing `color: var(--text)` or `font-family: var(--sans-font)` on a component, ask yourself: is this already inherited? Almost always, the answer is yes.

This also applies to properties that don't inherit by default but can be made to with the `inherit` keyword. For example, `button` and `select` elements don't inherit `font-family` and `font-size` from their parent by default — our base styles fix this with `font-size: inherit; font-family: inherit;` so you don't have to think about it in components.

---

## 6. Use rem and em — not px

Use `rem` for values that should scale with the user's base font size preference (spacing, font sizes, max-widths). Use `em` for values that should scale relative to the element's own font size — this is especially useful for icon sizing (`width: 1em; height: 1em` keeps an icon proportional to the text next to it), padding inside buttons, and component-internal spacing that should feel proportional regardless of context.

Avoid `px` for anything related to text, spacing, or layout. Pixels don't respect the user's font size settings, which is both an accessibility issue and a maintainability one — you can't scale a `px`-based UI by changing a single variable. Reserve `px` for things that genuinely shouldn't scale, like `1px` borders or box shadows.

---

## 7. Move shared styles out of scoped component CSS into global stylesheets

If more than one component uses the same pattern (collapsible details, form actions, button groups), it belongs in `main.scss` or `base.css`, not in `<style scoped>`.

---

## 8. Use correct heading levels

Heading levels should reflect document hierarchy, not visual size. Style headings with CSS, not by picking the heading level that looks right.

---

## 9. Use logical properties

Prefer `margin-block`, `padding-inline`, `inline-size`, `block-size` over their physical counterparts (`margin-top`, `padding-left`, `width`, `height`). This makes layouts more resilient to writing direction changes and is the modern CSS convention.
