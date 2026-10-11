#!/bin/bash
# Reads changed file paths (one per line) on stdin and prints which CI areas
# they touch, as GitHub Actions outputs: backend=true|false, frontend=true|false.
# See docs/ci.md for what each area covers and why e2e always runs.
set -euo pipefail

readonly SHARED_PATHS='^(\.editorconfig|\.gitattributes|\.github/workflows/ci\.yml|scripts/ci-changed-areas\.sh)$'
readonly BACKEND_PATHS='^(src/|tests/|analyzers/|tools/)|^(class-manager\.slnx|Directory\.Build\.props|Directory\.Packages\.props|NuGet\.config|dotnet-tools\.json|global\.json)$'
readonly FRONTEND_PATHS='^app/'

changed_files="$(cat)"

touches() {
  if grep -qE "$1|$SHARED_PATHS" <<< "$changed_files"; then echo true; else echo false; fi
}

echo "backend=$(touches "$BACKEND_PATHS")"
echo "frontend=$(touches "$FRONTEND_PATHS")"
