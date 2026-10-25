#!/bin/bash
# Usage: ./run-k6.sh <scenario file> <results name> [extra k6 arguments]
set -euo pipefail
readonly K6_IMAGE="grafana/k6:1.3.0"
readonly LOAD_TESTS_DIRECTORY="$(cd "$(dirname "$0")" && pwd)"
scenario="$1"
results_name="$2"
shift 2
mkdir -p "$LOAD_TESTS_DIRECTORY/.results"
docker run --rm --network host --user "$(id -u):$(id -g)" \
  --volume "$LOAD_TESTS_DIRECTORY":/loadtests --workdir /loadtests \
  --env LOAD_API_URL --env LOAD_PEAK_USERS --env LOAD_RAMP_SECONDS --env LOAD_HOLD_SECONDS --env LOAD_SIGN_INS_PER_SECOND \
  "$K6_IMAGE" run --quiet --out "csv=.results/$results_name.csv" "$@" "$scenario"
node "$LOAD_TESTS_DIRECTORY/report.mjs" "$LOAD_TESTS_DIRECTORY/.results/$results_name.csv"
