#!/bin/bash
# Waits until the newest revision of a container app is ready and answers /health, and fails the deploy
# otherwise. A revision that can't start (for example a setting the new code can't read) leaves the
# previous revision serving, so /health alone answers from the old code and the deploy looks green.
# Usage: wait-for-container-app-revision.sh <container app> <resource group>
set -euo pipefail

readonly CONTAINER_APP="$1"
readonly RESOURCE_GROUP="$2"
readonly ATTEMPTS=30
readonly DELAY_SECONDS=10
readonly HEALTH_PATH="/health"
readonly HEALTH_TIMEOUT_SECONDS=60

log() { echo "[wait-for-container-app-revision] $*" >&2; }

app_property() {
  az containerapp show --name "$CONTAINER_APP" --resource-group "$RESOURCE_GROUP" --query "$1" --output tsv
}

readonly LATEST_REVISION="$(app_property properties.latestRevisionName)"
readonly HEALTH_URL="https://$(app_property properties.configuration.ingress.fqdn)$HEALTH_PATH"
log "Waiting for $LATEST_REVISION"

ready_revision=""
for _ in $(seq 1 "$ATTEMPTS"); do
  curl --silent --output /dev/null --max-time "$HEALTH_TIMEOUT_SECONDS" "$HEALTH_URL" || true
  ready_revision="$(app_property properties.latestReadyRevisionName)"
  if [ "$ready_revision" = "$LATEST_REVISION" ]; then
    break
  fi
  sleep "$DELAY_SECONDS"
done

if [ "$ready_revision" != "$LATEST_REVISION" ]; then
  running_state="$(az containerapp revision show --name "$CONTAINER_APP" --resource-group "$RESOURCE_GROUP" \
    --revision "$LATEST_REVISION" --query properties.runningState --output tsv || true)"
  echo "::error::$LATEST_REVISION did not start (running state: ${running_state:-unknown}); $ready_revision still serves. See the revision's system logs in the portal."
  exit 1
fi

curl --fail --silent --show-error --retry 12 --retry-delay "$DELAY_SECONDS" --retry-all-errors \
  --max-time "$HEALTH_TIMEOUT_SECONDS" "$HEALTH_URL"
log "$LATEST_REVISION is ready and answers $HEALTH_PATH"
