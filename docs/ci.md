# Continuous integration

`.github/workflows/ci.yml` runs on every pull request to `main` and on every push to `main`. It has six jobs:

| Job | Checks | Runs on a pull request when it changes |
|---|---|---|
| `changes` | Decides which of the jobs below a pull request needs | always |
| `backend` | `dotnet build -warnaserror`, `dotnet test` | backend files |
| `backend-format` | `dotnet format --verify-no-changes` | backend files |
| `container` | Builds the API image (`src/Api/Containerfile`) | backend files |
| `frontend` | `pnpm lint`, `format:check`, `typecheck`, `knip`, `test` in `app/` | frontend files |
| `e2e` | Playwright against the web build, the real API and SQL Server | always |

A push to `main` runs every job, whatever it changed.

## Which files belong to each area

`scripts/ci-changed-areas.sh` reads the files a pull request changes and prints `backend=true|false` and `frontend=true|false`:

- **Backend**: `src/`, `tests/`, `analyzers/`, `tools/`, `class-manager.slnx`, `Directory.Build.props`, `Directory.Packages.props`, `NuGet.config`, `dotnet-tools.json`, `global.json`.
- **Frontend**: `app/`.
- **Both**: `.editorconfig` and `.gitattributes` (formatting of both sides), the workflow itself and the script.
- **Neither**: docs, `e2e/`, `scripts/` and anything else. Those pull requests still run `e2e`.

Try it locally:

```bash
git diff --name-only origin/main...HEAD | scripts/ci-changed-areas.sh
```

When a new top-level folder or root file feeds the build of one side, add it to the script.

## Why formatting is its own job

`dotnet format` loads the whole solution and runs the analyzers without needing a build, and takes about as long as the tests (around a minute). In its own job it runs in parallel with the build and the tests instead of before them, so a backend pull request finishes about a minute sooner. It catches the same problems (whitespace, imports, `IDE0005`, naming) as when it ran after the build.

## Why e2e always runs

The e2e tests cross the app and the API, so a change on either side, or in `e2e/` and the database scripts it uses, can break them. They are also the only check that runs on a pull request touching neither side. That is cheap insurance against a path the script does not know about.

## Why the jobs are skipped instead of the workflow

`backend`, `backend-format`, `frontend`, `e2e` and `container` are required status checks in the `main` ruleset (see [GitHub protection](github-protection.md)). A `paths` filter on the workflow would not start it at all, the required checks would stay "Expected" forever and the pull request could not be merged. A job skipped by its `if:` reports **Skipped**, which GitHub counts as passing for a required check.

The jobs only skip when `changes` says `false` explicitly (`needs.changes.outputs.<area> != 'false'`, with `!cancelled()`). If `changes` fails or its output is missing, every job runs, so a broken filter can never let an untested pull request through.

`changes` diffs the merge commit GitHub builds for the pull request against its first parent (`HEAD^1`, the current `main`), so it sees exactly what the pull request adds to `main`, even after `main` moves on. That needs only `fetch-depth: 2`.

The filter is a shell script because the repository only allows GitHub's own actions (`actions/*`), so path filter actions from the marketplace (such as `dorny/paths-filter`) can't be used.
