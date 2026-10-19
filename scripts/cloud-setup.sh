#!/bin/bash
# Prepares a Claude Code on the web container: .NET 10 SDK, frontend tooling,
# the Playwright browser for the e2e tests and the Azure CLI. The backend, the
# frontend and the Azure CLI are prepared in parallel. The SQL Server image is not pulled here: the SessionStart
# hook (cloud-session-start.sh) pulls it in the background when it is missing.
# Meant to be run from the cloud environment's Setup script. Idempotent.
set -euo pipefail

readonly PROJECT_DIRECTORY="$(cd "$(dirname "$0")/.." && pwd)"
readonly DOTNET_SDK_PACKAGE="dotnet-sdk-10.0"
readonly DOTNET_CHANNEL="10.0"
readonly DOTNET_USER_DIRECTORY="$HOME/.dotnet"
readonly FRONTEND_DIRECTORY="$PROJECT_DIRECTORY/app"
readonly E2E_DIRECTORY="$PROJECT_DIRECTORY/e2e"
readonly GIT_HOOKS_DIRECTORY=".githooks"
readonly PLAYWRIGHT_BROWSER="chromium"
readonly PLAYWRIGHT_DOWNLOAD_HOST="cdn.playwright.dev"
readonly AZURE_CLI_VERSION="2.91.0"
readonly AZURE_CLI_DIRECTORY="/opt/azure-cli"
readonly AZURE_CLI_LINK="/usr/local/bin/az"
readonly AZURE_CLI_PACKAGE_HOST="pypi.org"

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
  run_as_root apt-get -o DPkg::Lock::Timeout=300 update -qq || true
  if run_as_root env DEBIAN_FRONTEND=noninteractive apt-get -o DPkg::Lock::Timeout=300 install -y -qq "$DOTNET_SDK_PACKAGE"; then
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
    run_as_root env DEBIAN_FRONTEND=noninteractive apt-get -o DPkg::Lock::Timeout=300 install -y -qq nodejs npm
  fi
  log "Enabling pnpm through corepack (version pinned by packageManager)"
  persist_environment_variable COREPACK_ENABLE_DOWNLOAD_PROMPT "0"
  if ! corepack enable pnpm; then
    log "corepack enable failed, installing pnpm with npm"
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

install_playwright_browser() {
  if [ ! -f "$E2E_DIRECTORY/package.json" ]; then
    return
  fi
  log "Installing the Chromium revision pinned by the e2e Playwright version"
  (cd "$E2E_DIRECTORY" && pnpm exec playwright install "$PLAYWRIGHT_BROWSER") \
    || log "Playwright browser download failed, allow $PLAYWRIGHT_DOWNLOAD_HOST in the environment's network access"
}

azure_cli_is_installed() {
  [ -x "$AZURE_CLI_DIRECTORY/bin/az" ] \
    && "$AZURE_CLI_DIRECTORY/bin/pip" show azure-cli 2>/dev/null | grep -q "^Version: $AZURE_CLI_VERSION$"
}

install_azure_cli() {
  persist_environment_variable AZURE_CORE_COLLECT_TELEMETRY "0"
  if azure_cli_is_installed; then
    log "Azure CLI $AZURE_CLI_VERSION already installed"
  else
    log "Installing Azure CLI $AZURE_CLI_VERSION from PyPI into $AZURE_CLI_DIRECTORY"
    run_as_root python3 -m venv "$AZURE_CLI_DIRECTORY"
    run_as_root "$AZURE_CLI_DIRECTORY/bin/pip" install --quiet "azure-cli==$AZURE_CLI_VERSION"
  fi
  run_as_root ln -sf "$AZURE_CLI_DIRECTORY/bin/az" "$AZURE_CLI_LINK"
}

prepare_azure_cli() {
  install_azure_cli || log "Azure CLI install failed, allow $AZURE_CLI_PACKAGE_HOST and files.pythonhosted.org in the environment's network access"
}

enable_git_hooks() {
  log "Enabling repository git hooks ($GIT_HOOKS_DIRECTORY)"
  git -C "$PROJECT_DIRECTORY" config core.hooksPath "$GIT_HOOKS_DIRECTORY"
}

prepare_backend() {
  install_dotnet
  restore_dotnet_packages
}

prepare_frontend() {
  install_frontend_tooling
  install_playwright_browser
}

enable_git_hooks
configure_dotnet_environment
prepare_backend &
readonly BACKEND_PROCESS_ID=$!
prepare_frontend &
readonly FRONTEND_PROCESS_ID=$!
prepare_azure_cli &
readonly AZURE_CLI_PROCESS_ID=$!
wait "$BACKEND_PROCESS_ID"
wait "$FRONTEND_PROCESS_ID"
wait "$AZURE_CLI_PROCESS_ID"
log "Done"
