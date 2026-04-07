# Copilot Instructions

## Project Guidelines
- When specifying a bundle in a Gherkin step as a table, omit the header row (element_name | expected_value) since a bundle is always a 2-column table of element name and value pairs — no header row is needed.
- When writing or updating Gherkin feature files, prefer the reusable bundle-based steps in `generalized_steps.py` (verify, populate, visibility) over creating feature-specific step definitions. Only create a feature-specific step when no existing generalized step can handle the interaction.
- When implementing a new step definition, consider whether it could be written as a generalized step that works with the Page Object Model (`app_page.py`) and the element-name/value bundle pattern. If so, add it to `generalized_steps.py` rather than a feature-specific step file.