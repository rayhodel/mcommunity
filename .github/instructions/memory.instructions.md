---
applyTo: '**'
---
# GitHub Copilot Instructions

These instructions guide GitHub Copilot to generate proper documentation.  It also specifies the shell/terminal commands to use. 

## Documentation Expectations

- For source file structure changes:
  - Document the rationale behind creating, modifying, or deleting source files.
  - Explain the purpose of each file and its role within the overall project structure.

- For libraries, packages, or dependencies added or updated:
  - Describe why the library/package is used.
  - Provide example usage and any configuration details.

- For functions, classes, and modules:
  - Include inline comments summarizing inputs, outputs, and logic.
  - Add usage examples and note any design decisions when relevant.

## Structural and Process Documentation

- Update or maintain a high-level architectural overview when project structure changes significantly.
- Document build, test, and deployment scripts detailing commands, sequence, required tools/versions, and environment setup.
- Keep documentation for CI/CD workflows and processes current with any pipeline modifications.

## Best Practices

- Ensure documentation is clear, consistent, and uses Markdown formatting.
- Use active voice and present tense.
- Check documentation completeness and accuracy before finalizing code changes.
- Flag any missing or outdated documentation for review.

## Terminal Commands

- Always use Powershell commands and formatting for terminal commands.

## Memory
You have a memory that stores information about the user and their preferences. This memory is used to provide a more personalized experience. You can access and update this memory as needed. The memory is stored in a file called `.github/instructions/memory.instructions.md`. If the file is empty, you'll need to create it.

When creating a new memory file, you MUST include the following front matter at the top of the file:
```yaml
---
applyTo: '**'
---
```

If the user asks you to remember something or add something to your memory, you can do so by updating the memory file.

## Reference docs/DOCUMENTATION-{{feature}}.md

- Cross-reference or update `docs/DOCUMENTATION-{{feature}}.md` after every significant change to keep the main documentation file aligned.
- Ensure `docs/DOCUMENTATION-{{feature}}.md` covers features, file structure, and usage guidelines as per above requirements.


## Reference docs/updates

- Any updates that are documented need to be written to the `docs/updates/` folder with a file name that reflects the update made. For example, if you updated the CSS classes used in the application, you would create a file called `docs/updates/CSS-CLASS-MIGRATION.md` and document the changes made.

## Maintain a CHANGELOG.md

- Maintain a changelog section in `docs/CHANGELOG.md` to track milestones and significant updates and modifications over time.

## Task Management

Follow the format outlines in the `docs/task-instructions.md` file for creating and managing tasks.
