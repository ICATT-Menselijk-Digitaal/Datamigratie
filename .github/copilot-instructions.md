# GitHub Copilot PR Review Instructions
When reviewing a pull request, use the following checklist and guidelines:
- The changelog (docs/changelog/changelog.md) should contain an entry for the story related to this pull request. The entry should be in the 'Current version' section of the changelog. The entry should consist of the title of the story and a link to the story in Jira. Usually the link has this format: 'https://dimpact.atlassian.net/browse/[DATA-XXX]'. The right value for [DATA-XXX] can be found in the name of the current branch.
- Feature isolation: Code belongs in feature folder, not spread across shared locations unless truly reusable
- Avoid premature abstraction: Only add interfaces if mocking or multiple implementations are needed
- Avoid duplication: Abstract repeated patterns into shared functions
- Use GUIDs over URLs: For identifiers, extract GUIDs rather than storing full URLs (environment safety)
- Meaningful defaults: Don't use zero/dummy values when "not computed" differs from "zero" - use nullable
- Use GUIDs over URLs: For identifiers, extract GUIDs rather than storing full URLs (environment safety)
- Explicit null handling: Distinguish "not found" (return null) from errors (throw exception)
- Proper status codes: Use 404 for not found, 400 for validation errors, 409 for conflicts
- API client pattern: Return null on 404, then call `EnsureSuccessStatusCode()` for other errors
- Don't expose raw exceptions: Be intentional about what error messages reach the API response
- Defensive deserialization: Handle unexpected null/empty responses from external APIs
## Patterns
- Vertical Slice / Feature-Driven Design: Each feature is self-contained in `/Features/[Feature]/[SubFeature]/` with its own controller, service, and models.
- Primary constructor injection: `public class Service(IDependency dep) : IService`
- Interface-first for services: `IMapZaaktypenService` + `MapZaaktypenService` (in the same file)
- Async/await throughout all I/O operations
- EF Core with snake_case naming (via EFCore.NamingConventions)
## Frontend
- Reuse helper functions: Share utilities like date formatting across views
- Native form validation: Use `setCustomValidity()` instead of custom validation state
- Consistent styling: Use CSS custom properties, don't create per-page custom styles
- Consistent behavior: Error/success messages should work the same way across pages
## Data Types & Models
- Use proper types: `Guid` instead of `string` for UUIDs
- Complete model mapping: Ensure all required API fields are mapped correctly
## Database & Migrations
- Convention over configuration: Trust EF Core conventions instead of explicit configuration when the convention produces the correct result. Don't add `.ToTable()`, `.HasColumnName()`, or other fluent API calls that just repeat what EF would infer automatically. Less code = less noise = fewer bugs.
- Migration impact: Editing existing migrations requires all developers to recreate their database
- Constraint design: Consider relationship cardinality (one-to-one vs one-to-many) with stakeholders
## Dependencies & Configuration
- Version alignment: Keep framework versions consistent (SDK matching runtime)
- Stable port configuration: Consider fixed ports for local development convenience
## Testing
- Test via public interfaces only: Do not unit test private methods. Test behavior by observing public outputs (return values, DB state, exceptions) given controlled inputs.
## Language Notes
- Code comments and technical discussions: English preferred
- UI strings and user-facing content: Dutch (Netherlands)
- PR discussions: Mix of Dutch and English is acceptable
