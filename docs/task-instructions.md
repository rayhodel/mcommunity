
# Project Task Instructions

## Introduction
This document provides guidelines on how to structure and document tasks for the Space Invaders .NET Core MVC project. Each task should be detailed in its own markdown file within the `tasks/` directory, following a consistent format to ensure clarity and ease of tracking progress.

## Task Files

Organize your tasks in a structured format using the following files

`tasks/Task <number> - <Task Name>.md`:

```markdown
# Task <number> - <Task Name>

<description>
## Status: <In Progress/Not Started/Completed/On Hold>
## Dependencies: <List of dependent tasks>
## Steps to Complete:
[~] 1. <Step 1>
   [x] 1.1. <Sub-step 1>
   [ ] 1.2. <Sub-step 2>
[ ] 2. <Step 2>
...
## Completion Notes:
Task 1 completed on <date>. <Details of completion>
Task 1.1 completed on <date>. <Details of completion>
...

```

### Example Files

<example>
- `tasks/Task 1 - Initialize Project.md`: Contains details about the initial setup tasks for the project.
- `tasks/Task 2 - Configure and Seed Database.md`: Contains tasks related to database configuration and seeding initial data.
- `tasks/Task 3 - Implement Authentication.md`: Contains tasks for setting up user authentication.
</example>

## Marking Individual Tasks

Inside the task files is a list of tasks to perform.  They should have a box with a symbol next to it to identify the status.  These are the status types:

[ ] - Not started
[~] - In Progress
[x] - Completed
[!] - On Hold

## In Progress Tasks

When a task is in progress it must be marked as `In Progress`

## Completed Tasks

When a task is completed it must be marked as `Completed`

## Workflow

1. Check if there are tasks to perform that are not `Completed`
2. Any tasks that are not `Completed` need to be done in order.  keep in mind that tasks are numbered and include dependencies.
3. After a task is worked on always update the `Status`


