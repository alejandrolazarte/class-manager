#!/bin/bash
# Runs at the start of every Claude Code on the web session (SessionStart hook in
# .claude/settings.json). Processes started by cloud-setup.sh do not survive into
# the session, so this starts the Docker daemon and the SQL Server container used
# by the e2e tests, and exports E2E_DATABASE_CONNECTION_STRING. The daemon and the
# container (pulling the image when it is missing) start in the background so the
# session does not wait for them; --wait runs in the foreground until SQL Server
# is ready. When the environment has the read-only audit service principal
# (AZURE_CLIENT_ID, AZURE_CLIENT_SECRET, AZURE_TENANT_ID), it also signs the
# Azure CLI in with it. Does nothing on developer machines. Idempotent.
set -euo pipefail

readonly SQL_SERVER_IMAGE="mcr.microsoft.com/mssql/server:2022-latest"
readonly SQL_SERVER_CONTAINER="class-manager-sql"
readonly SQL_SERVER_PORT="1433"
readonly SQL_SERVER_THROWAWAY_PASSWORD="CloudE2e_Passw0rd!"
readonly SQL_SERVER_READY_MESSAGE="SQL Server is now ready for client connections"
readonly SQL_SERVER_READY_TIMEOUT_SECONDS=90
readonly E2E_DATABASE_NAME="ClassManagerE2E"
readonly DOCKER_LOG_FILE="/tmp/dockerd.log"
readonly DOCKER_READY_TIMEOUT_SECONDS=30
readonly BACKGROUND_LOG_FILE="/tmp/cloud-session-start.log"
readonly WAIT_ARGUMENT="--wait"
readonly AZURE_SIGN_IN_LOG_FILE="/tmp/azure-sign-in.log"

log() { echo "[cloud-session-start] $*" >&2; }

if [ "${CLAUDE_CODE_REMOTE:-}" != "true" ]; then
  exit 0
fi

persist_environment_variable() {
  if [ -n "${CLAUDE_ENV_FILE:-}" ]; then
    echo "export $1=\"$2\"" >> "$CLAUDE_ENV_FILE"
  fi
  export "$1=$2"
}

run_as_root() {
  if [ "$(id -u)" -eq 0 ]; then "$@"; else sudo "$@"; fi
}

docker_daemon_is_running() {
  docker info >/dev/null 2>&1
}

start_docker_daemon() {
  if docker_daemon_is_running; then
    return 0
  fi
  if ! command -v dockerd >/dev/null 2>&1; then
    log "dockerd not available, SQL Server will not start"
    return 1
  fi

  log "Starting Docker daemon"
  run_as_root bash -c "nohup dockerd > '$DOCKER_LOG_FILE' 2>&1 &"
  for _ in $(seq 1 "$DOCKER_READY_TIMEOUT_SECONDS"); do
    if docker_daemon_is_running; then
      return 0
    fi
    sleep 1
  done
  log "Docker daemon did not start, see $DOCKER_LOG_FILE"
  return 1
}

sql_server_container_exists() {
  docker container inspect "$SQL_SERVER_CONTAINER" >/dev/null 2>&1
}

start_sql_server() {
  if sql_server_container_exists; then
    docker start "$SQL_SERVER_CONTAINER" >/dev/null
  else
    log "Creating $SQL_SERVER_CONTAINER"
    docker run --detach --name "$SQL_SERVER_CONTAINER" \
      --env ACCEPT_EULA=Y \
      --env "MSSQL_SA_PASSWORD=$SQL_SERVER_THROWAWAY_PASSWORD" \
      --publish "$SQL_SERVER_PORT:$SQL_SERVER_PORT" \
      "$SQL_SERVER_IMAGE" >/dev/null
  fi

  local started_at
  started_at="$(docker container inspect --format '{{.State.StartedAt}}' "$SQL_SERVER_CONTAINER")"
  for _ in $(seq 1 "$SQL_SERVER_READY_TIMEOUT_SECONDS"); do
    if docker logs --since "$started_at" "$SQL_SERVER_CONTAINER" 2>&1 | grep -q "$SQL_SERVER_READY_MESSAGE"; then
      log "SQL Server ready on port $SQL_SERVER_PORT"
      return 0
    fi
    sleep 1
  done
  log "SQL Server did not report ready, see docker logs $SQL_SERVER_CONTAINER"
  return 1
}

export_e2e_connection_string() {
  persist_environment_variable E2E_DATABASE_CONNECTION_STRING \
    "Server=tcp:localhost,$SQL_SERVER_PORT;Database=$E2E_DATABASE_NAME;User Id=sa;Password=$SQL_SERVER_THROWAWAY_PASSWORD;TrustServerCertificate=True"
}

start_docker_and_sql_server() {
  start_docker_daemon && start_sql_server
}

azure_service_principal_is_configured() {
  [ -n "${AZURE_CLIENT_ID:-}" ] && [ -n "${AZURE_CLIENT_SECRET:-}" ] && [ -n "${AZURE_TENANT_ID:-}" ]
}

sign_in_to_azure() {
  if ! command -v az >/dev/null 2>&1 || ! azure_service_principal_is_configured; then
    return 0
  fi
  log "Signing the Azure CLI in as $AZURE_CLIENT_ID in the background, see $AZURE_SIGN_IN_LOG_FILE"
  az login --service-principal --username "$AZURE_CLIENT_ID" --password="$AZURE_CLIENT_SECRET" \
    --tenant "$AZURE_TENANT_ID" --allow-no-subscriptions --output none </dev/null >>"$AZURE_SIGN_IN_LOG_FILE" 2>&1 &
  disown
}

export_e2e_connection_string
sign_in_to_azure
if [ "${1:-}" = "$WAIT_ARGUMENT" ]; then
  start_docker_and_sql_server || exit 1
else
  log "Starting Docker and SQL Server in the background, see $BACKGROUND_LOG_FILE"
  start_docker_and_sql_server </dev/null >>"$BACKGROUND_LOG_FILE" 2>&1 &
  disown
fi
exit 0
