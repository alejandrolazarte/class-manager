#!/bin/bash
# Prepares a Claude Code on the web container: .NET 10 SDK, frontend tooling,
# a container engine for Testcontainers and the SQL Server image it uses.
# Meant to be run from the cloud environment's Setup script. Idempotent.
set -euo pipefail

readonly PROJECT_DIRECTORY="$(cd "$(dirname "$0")/.." && pwd)"
readonly DOTNET_SDK_PACKAGE="dotnet-sdk-10.0"
readonly DOTNET_CHANNEL="10.0"
readonly DOTNET_USER_DIRECTORY="$HOME/.dotnet"
readonly SQL_SERVER_IMAGE="mcr.microsoft.com/mssql/server:2022-latest"
readonly DOCKER_SOCKET="/var/run/docker.sock"
readonly DOCKER_LOG_FILE="/tmp/dockerd.log"
readonly FRONTEND_DIRECTORY="$PROJECT_DIRECTORY/app"
readonly E2E_DIRECTORY="$PROJECT_DIRECTORY/e2e"
readonly GIT_HOOKS_DIRECTORY=".githooks"

log() { echo "[cloud-setup] $*" >&2; }

persist_environment_variable() {
  if [ -n "${CLAUDE_ENV_FILE:-}" ]; then
    echo "export $1=\"$2\"" >> "$CLAUDE_ENV_FILE"
  fi
  export "$1=$2"
}

run_as_root() {
  if [ "$(id -u)" -eq 0 ]; then "$@"; else sudo "$@"; fi
}

dotnet_10_is_installed() {
  command -v dotnet >/dev/null 2>&1 && dotnet --list-sdks 2>/dev/null | grep -q "^10\."
}

install_dotnet() {
  if dotnet_10_is_installed; then
    log ".NET 10 SDK already installed"
    return
  fi

  log "Installing .NET 10 SDK from apt"
  run_as_root apt-get update -qq || true
  if run_as_root env DEBIAN_FRONTEND=noninteractive apt-get install -y -qq "$DOTNET_SDK_PACKAGE"; then
    return
  fi

  log "apt install failed, falling back to dotnet-install.sh"
  curl -sSfL https://dot.net/v1/dotnet-install.sh -o /tmp/dotnet-install.sh
  bash /tmp/dotnet-install.sh --channel "$DOTNET_CHANNEL" --install-dir "$DOTNET_USER_DIRECTORY"
  persist_environment_variable DOTNET_ROOT "$DOTNET_USER_DIRECTORY"
  persist_environment_variable PATH "$DOTNET_USER_DIRECTORY:$PATH"
}

configure_dotnet_environment() {
  persist_environment_variable DOTNET_CLI_TELEMETRY_OPTOUT "1"
  persist_environment_variable DOTNET_NOLOGO "1"
  persist_environment_variable DOTNET_SKIP_FIRST_TIME_EXPERIENCE "1"
}

restore_dotnet_packages() {
  log "Restoring NuGet packages"
  dotnet restore "$PROJECT_DIRECTORY/class-manager.slnx"
}

install_frontend_tooling() {
  if ! command -v node >/dev/null 2>&1; then
    log "Node.js not found, installing from apt"
    run_as_root env DEBIAN_FRONTEND=noninteractive apt-get install -y -qq nodejs npm
  fi
  if ! command -v pnpm >/dev/null 2>&1; then
    log "Installing pnpm"
    npm install -g pnpm
  fi

  if [ -f "$FRONTEND_DIRECTORY/package.json" ]; then
    log "Installing frontend dependencies"
    (cd "$FRONTEND_DIRECTORY" && pnpm install)
  else
    log "No app/package.json yet, skipping frontend dependencies"
  fi

  if [ -f "$E2E_DIRECTORY/package.json" ]; then
    log "Installing e2e dependencies"
    (cd "$E2E_DIRECTORY" && pnpm install)
  fi
}

docker_daemon_is_running() {
  docker info >/dev/null 2>&1
}

start_container_engine() {
  if docker_daemon_is_running; then
    log "Docker daemon already running"
    return
  fi
  if ! command -v dockerd >/dev/null 2>&1; then
    log "dockerd not available, integration tests will not run"
    return
  fi

  log "Starting Docker daemon"
  run_as_root bash -c "nohup dockerd > '$DOCKER_LOG_FILE' 2>&1 &"
  for _ in $(seq 1 30); do
    if docker_daemon_is_running; then
      return
    fi
    sleep 1
  done
  log "Docker daemon did not start, see $DOCKER_LOG_FILE"
}

configure_testcontainers() {
  persist_environment_variable DOCKER_HOST "unix://$DOCKER_SOCKET"
}

pull_sql_server_image() {
  if ! docker_daemon_is_running; then
    return
  fi
  if docker image inspect "$SQL_SERVER_IMAGE" >/dev/null 2>&1; then
    log "SQL Server image already present"
    return
  fi
  log "Pulling $SQL_SERVER_IMAGE"
  docker pull "$SQL_SERVER_IMAGE"
}

enable_git_hooks() {
  log "Enabling repository git hooks ($GIT_HOOKS_DIRECTORY)"
  git -C "$PROJECT_DIRECTORY" config core.hooksPath "$GIT_HOOKS_DIRECTORY"
}

enable_git_hooks
install_dotnet
configure_dotnet_environment
restore_dotnet_packages
install_frontend_tooling
start_container_engine
configure_testcontainers
pull_sql_server_image
log "Done"
