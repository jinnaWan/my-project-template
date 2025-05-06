# CI/CD Pipeline for MyProject

This repository uses GitHub Actions for continuous integration and continuous deployment.

## Workflows

### Frontend Tests

The `frontend-tests.yml` workflow runs whenever there are changes to files in the `MyProject.Client` directory on pull requests to the `main` branch or direct pushes to `main`.

This workflow:
1. Checks out the code
2. Sets up Node.js
3. Installs dependencies
4. Runs linting checks
5. Runs tests with coverage
6. Uploads the coverage report as an artifact

### Branch Protection

The `branch-protection.yml` workflow sets up branch protection rules for the `main` branch.

It requires:
1. Pull request reviews (at least 1 approval)
2. Status checks to pass (specifically the "Run Frontend Tests" check)
3. No direct pushes to `main` without pull requests

## Setting Up Required Secrets

For the branch protection workflow to work properly, you need to make sure the GitHub token has sufficient permissions.

## Pull Request Process

1. Create a new branch for your feature or bugfix
2. Make changes and commit them
3. Push the branch to GitHub
4. Create a pull request to the `main` branch
5. Wait for the CI checks to pass
6. Get a code review and approval
7. Once approved and all checks pass, the pull request can be merged 