# GitHub Copilot PR Review Instructions

When reviewing a pull request, use the following checklist and guidelines.

## PR Review Checklist

- The changelog (docs/changelog/changelog.md) should contain an entry for the story related to this pull request. The entry should be in the 'Current version' section of the changelog. The entry should consist of the title of the story and a link to the story in Jira. Usually the link has this format: 'https://dimpact.atlassian.net/browse/[DATA-XXX]'. The right value for [DATA-XXX] can be found in the name of the current branch.
- Feature isolation: Code belongs in feature folder, not spread across shared locations unless truly reusable
- Avoid premature abstraction: Only add interfaces if mocking or multiple implementations are needed
- Avoid duplication: Abstract repeated patterns into shared functions
- Use GUIDs over URLs: For identifiers, extract GUIDs rather than storing full URLs (environment safety)
- Meaningful defaults: Don't use zero/dummy values when "not computed" differs from "zero" - use nullable
- Generate a comment when a field is removed from "zaak mapping" 
