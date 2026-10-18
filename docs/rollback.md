# Rolling back a deploy

Date: 2026-10-18

There is one environment, the pilot, and every merge to `main` deploys to it by itself ([Automatic deploys](pilot-deployment.md#automatic-deploys)). There is no staging: CI tests every pull request against a real SQL Server and runs the e2e tests, and when a deploy still breaks something, the **Rollback** workflow puts a version that worked back online in a few minutes.

## What a rollback does

`.github/workflows/rollback.yml`, started by hand from GitHub Actions:

| Piece | What happens |
|---|---|
| API | The container app goes back to the image of the chosen commit, already in `ghcr.io` (nothing is built) |
| Web app | The web app of the chosen commit is built and uploaded to Static Web Apps; installed PWAs pick it up the next time they open |
| Database | Untouched: no migration runs, the schema stays as the latest deploy left it and data entered since then is kept |
| `main` | Untouched: the broken change is still there and the next merge deploys it again |
| Android APK | Nothing to do: it calls the same API |

It only offers versions that the **Deploy** workflow deployed successfully (its `api` job passed), so a typo, a branch or a commit that never ran in production can't be deployed. It reads the run history, so it has no age limit, unlike re-running an old Deploy run (GitHub allows re-runs for 30 days).

## Steps

1. GitHub → **Actions** → **Rollback** → **Run workflow**, from `main`, with `list`. Nothing is deployed; the run summary shows the last 10 successful deploys, numbered from the most recent, with the pull request and the commit.
2. **Run workflow** again with the number of the version to go back to. If the broken deploy finished green, it is number 1 and the one before it is number 2; if it failed, it is not listed.
3. Check the run summary: **Rolling back to** names the version that was deployed.
4. Revert or fix the change on `main` through a pull request before merging anything else.
5. Check whether the broken version wrote wrong data; fix it by hand, or in the worst case restore the database to a point in time from the Azure portal (it restores to a new database and loses what was entered after that point).

The list is read again when the rollback runs: if a deploy finishes between steps 1 and 2, the numbers move by one, so don't merge in between, and check step 3.

## Who can run it

Only accounts with write access to the repository see **Run workflow**. Azure trusts only runs from `main` of this repository (the federated credential subject in [Pilot deployment](pilot-deployment.md)), so a fork or another branch can't sign in; the workflow stops early when it is not run from `main`. Rollback and Deploy share the `deploy-pilot` concurrency group, so they never deploy at the same time; `list` runs outside it.

## Keep migrations backward compatible

The previous image runs against the schema of the latest deploy, so every migration must leave the database usable by the version before it:

- Add tables and columns; new columns are nullable or have a default.
- Dropping or renaming something takes two deploys: first a pull request stops using it, then a later one removes it.

A migration that breaks this can't be rolled back with this workflow; fix it with a new migration.

## When to add staging

When there are paying customers and downtime costs money or trust, when more people work on the code and want to try changes together before they go out, or when integrations that CI can't test arrive (real payments, webhooks).
