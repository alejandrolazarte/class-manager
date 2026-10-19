#!/bin/bash
# Sets up the audit log storage account (docs/audit-log-storage.md) from Azure
# Cloud Shell, signed in as the owner: checks the account settings, creates the
# security-events container and the Audit Log Writer role, proves the role with
# a temporary service principal, then gives it to the API's managed identity
# and the reader roles to the owner. Stops before touching the API if any check
# fails. Idempotent: run it again if the Cloud Shell session closes. Everything
# it prints is also appended to ~/setup-audit-log-storage.log, which survives
# the session.
set -uo pipefail

readonly RESOURCE_GROUP="class-manager-rg"
readonly CONTAINER_APP="class-manager-api"
readonly AUDIT_ACCOUNT="classmanageraudit"
readonly SECURITY_EVENTS_CONTAINER="security-events"
readonly ROLE_CHECK_CONTAINER="role-check"
readonly ROLE_CHECK_TABLE="RoleCheck"
readonly WRITER_ROLE="Audit Log Writer"
readonly TABLE_READER_ROLE="Storage Table Data Reader"
readonly BLOB_READER_ROLE="Storage Blob Data Reader"
readonly CHECK_PRINCIPAL_NAME="class-manager-audit-role-check"
readonly STORAGE_RESOURCE="https://storage.azure.com"
readonly STORAGE_API_VERSION="2023-11-03"
readonly MANAGEMENT_API_VERSION="2023-05-01"
readonly PROPAGATION_TIMEOUT_SECONDS=600
readonly APPLIED_ROLE_TIMEOUT_SECONDS=120
readonly LOG_FILE="$HOME/setup-audit-log-storage.log"
readonly RETRY_DELAY_SECONDS=30
readonly FORBIDDEN_PATTERN="AuthorizationPermissionMismatch|AuthorizationFailure|Forbidden|403|do not have the required permissions|UnauthorizedBlobOverwrite"

CHECK_CONFIG_DIRECTORY="$(mktemp -d)"
RUN_ID="$(date +%s)"
readonly CHECK_CONFIG_DIRECTORY RUN_ID
readonly BLOB_URL="https://$AUDIT_ACCOUNT.blob.core.windows.net/$ROLE_CHECK_CONTAINER/$RUN_ID.jsonl"
readonly TABLE_ARGUMENTS=(--account-name "$AUDIT_ACCOUNT" --auth-mode login)
readonly BLOB_ARGUMENTS=(--resource "$STORAGE_RESOURCE" --headers "x-ms-version=$STORAGE_API_VERSION")

check_app_id=""
check_object_id=""
failures=0
role_applied=false

exec > >(tee -a "$LOG_FILE") 2>&1
printf '\n######## %s\n' "$(date -u '+%Y-%m-%d %H:%M:%S UTC')"

step() { printf '\n== %s\n' "$*"; }
info() { printf '   %s\n' "$*"; }
fail() { printf '\nERROR: %s\n' "$*" >&2; exit 1; }
as_checker() { AZURE_CONFIG_DIR="$CHECK_CONFIG_DIRECTORY" "$@"; }

assign_role() {
  local object_id="$1" principal_type="$2" role="$3"
  local deadline=$((SECONDS + PROPAGATION_TIMEOUT_SECONDS))
  local existing
  existing="$(az role assignment list --scope "$AUDIT_ACCOUNT_ID" --role "$role" \
    --query "[?principalId=='$object_id'] | length(@)" -o tsv 2>/dev/null)"
  if [ "${existing:-0}" != "0" ]; then
    info "already has $role"
    return
  fi
  until az role assignment create --assignee-object-id "$object_id" --assignee-principal-type "$principal_type" \
    --role "$role" --scope "$AUDIT_ACCOUNT_ID" -o none 2>/dev/null; do
    [ "$SECONDS" -lt "$deadline" ] || fail "Could not assign $role"
    info "waiting for $role to be assignable..."
    sleep "$RETRY_DELAY_SECONDS"
  done
  info "assigned $role"
}

remove_check_principals() {
  local app_id object_id
  for app_id in $(az ad app list --display-name "$CHECK_PRINCIPAL_NAME" --query "[].appId" -o tsv); do
    object_id="$(az ad sp show --id "$app_id" --query id -o tsv 2>/dev/null)"
    if [ -n "$object_id" ]; then
      az role assignment delete --assignee "$object_id" --scope "$AUDIT_ACCOUNT_ID" -o none 2>/dev/null
    fi
    az ad app delete --id "$app_id" -o none
    info "removed service principal $app_id"
  done
}

cleanup() {
  step "Removing the temporary service principal and its test data"
  rm -rf "$CHECK_CONFIG_DIRECTORY"
  remove_check_principals
  az storage container-rm delete --storage-account "$AUDIT_ACCOUNT" -g "$RESOURCE_GROUP" \
    -n "$ROLE_CHECK_CONTAINER" --yes -o none 2>/dev/null
  az rest --method delete -o none 2>/dev/null \
    --url "https://management.azure.com$AUDIT_ACCOUNT_ID/tableServices/default/tables/$ROLE_CHECK_TABLE?api-version=$MANAGEMENT_API_VERSION"
}

outcome_of() {
  local output
  if output="$(as_checker "$@" 2>&1)"; then
    echo "allowed"
  elif grep -qE "$FORBIDDEN_PATTERN" <<< "$output"; then
    echo "forbidden"
  else
    echo "error: $(tr '\n' ' ' <<< "$output" | cut -c1-300)"
  fi
}

expect() {
  local expected="$1" description="$2"
  shift 2
  local actual
  actual="$(outcome_of "$@")"
  if [ "$actual" = "$expected" ]; then
    printf '   ok    %-24s %s\n' "$description" "$expected"
  else
    printf '   FAIL  %-24s expected %s, got %s\n' "$description" "$expected" "$actual"
    failures=$((failures + 1))
  fi
}

expect_allowed_once_applied() {
  local description="$1"
  shift
  local timeout_seconds="$PROPAGATION_TIMEOUT_SECONDS"
  if [ "$role_applied" = "true" ]; then
    timeout_seconds="$APPLIED_ROLE_TIMEOUT_SECONDS"
  fi
  local deadline=$((SECONDS + timeout_seconds))
  until [ "$(outcome_of "$@")" = "allowed" ]; do
    if [ "$SECONDS" -ge "$deadline" ]; then
      expect allowed "$description" "$@"
      return
    fi
    info "waiting for the role to apply ($description)..."
    sleep "$RETRY_DELAY_SECONDS"
  done
  role_applied=true
  printf '   ok    %-24s %s\n' "$description" "allowed"
}

SUBSCRIPTION_ID="$(az account show --query id -o tsv)" || fail "Sign in with az login first"
AUDIT_ACCOUNT_ID="$(az storage account show -n "$AUDIT_ACCOUNT" -g "$RESOURCE_GROUP" --query id -o tsv)" \
  || fail "Storage account $AUDIT_ACCOUNT not found in $RESOURCE_GROUP"
readonly SUBSCRIPTION_ID AUDIT_ACCOUNT_ID

step "Checking the settings of $AUDIT_ACCOUNT"
account_settings="$(az storage account show -n "$AUDIT_ACCOUNT" -g "$RESOURCE_GROUP" \
  --query "join(' ', [to_string(allowSharedKeyAccess), to_string(allowBlobPublicAccess), minimumTlsVersion])" -o tsv)"
info "account key access, anonymous access, minimum TLS: $account_settings"
[ "$account_settings" = "false false TLS1_2" ] || fail "Expected 'false false TLS1_2'. Fix the account before going on"

step "Security events container"
if [ "$(az storage container-rm exists --storage-account "$AUDIT_ACCOUNT" -g "$RESOURCE_GROUP" \
  -n "$SECURITY_EVENTS_CONTAINER" --query exists -o tsv)" = "true" ]; then
  info "$SECURITY_EVENTS_CONTAINER already exists"
else
  az storage container-rm create --storage-account "$AUDIT_ACCOUNT" -g "$RESOURCE_GROUP" \
    -n "$SECURITY_EVENTS_CONTAINER" --public-access off -o none || fail "Could not create $SECURITY_EVENTS_CONTAINER"
  info "created $SECURITY_EVENTS_CONTAINER"
fi

step "Role $WRITER_ROLE"
role_definition="$(cat <<JSON
{
  "Name": "$WRITER_ROLE",
  "Description": "Adds audit log entries: creates tables, inserts rows, creates blobs and appends to them. Can't read, update or delete.",
  "Actions": ["Microsoft.Storage/storageAccounts/tableServices/tables/write"],
  "NotActions": [],
  "DataActions": [
    "Microsoft.Storage/storageAccounts/tableServices/tables/entities/add/action",
    "Microsoft.Storage/storageAccounts/blobServices/containers/blobs/add/action"
  ],
  "NotDataActions": [],
  "AssignableScopes": ["/subscriptions/$SUBSCRIPTION_ID/resourceGroups/$RESOURCE_GROUP"]
}
JSON
)"
if [ "$(az role definition list --name "$WRITER_ROLE" --custom-role-only true --query "length(@)" -o tsv)" = "0" ]; then
  az role definition create --role-definition "$role_definition" -o none || fail "Could not create $WRITER_ROLE"
  info "created"
else
  current_permissions="$(az role definition list --name "$WRITER_ROLE" --custom-role-only true \
    --query "join(' ', sort([0].permissions[0].[actions, dataActions][]))" -o tsv)"
  expected_permissions="$(jq -r '[.Actions, .DataActions] | flatten | sort | join(" ")' <<< "$role_definition")"
  [ "$current_permissions" = "$expected_permissions" ] \
    || fail "$WRITER_ROLE exists with other permissions: $current_permissions. Fix it with az role definition update"
  info "already exists with the expected permissions"
fi

step "Temporary service principal with only $WRITER_ROLE"
remove_check_principals
check_credentials="$(az ad sp create-for-rbac --name "$CHECK_PRINCIPAL_NAME" --only-show-errors -o json)" \
  || fail "Could not create the temporary service principal"
trap cleanup EXIT
check_app_id="$(jq -r .appId <<< "$check_credentials")"
check_password="$(jq -r .password <<< "$check_credentials")"
check_tenant="$(jq -r .tenant <<< "$check_credentials")"
check_object_id="$(az ad sp show --id "$check_app_id" --query id -o tsv)"
assign_role "$check_object_id" ServicePrincipal "$WRITER_ROLE"
az storage container-rm create --storage-account "$AUDIT_ACCOUNT" -g "$RESOURCE_GROUP" \
  -n "$ROLE_CHECK_CONTAINER" --public-access off -o none || fail "Could not create $ROLE_CHECK_CONTAINER"
deadline=$((SECONDS + PROPAGATION_TIMEOUT_SECONDS))
until as_checker az login --service-principal --username "$check_app_id" --password="$check_password" \
  --tenant "$check_tenant" --allow-no-subscriptions -o none 2>/dev/null; do
  [ "$SECONDS" -lt "$deadline" ] || fail "The temporary service principal could not sign in"
  info "waiting for the service principal to be able to sign in..."
  sleep "$RETRY_DELAY_SECONDS"
done

step "Proving the role (the first check waits up to $((PROPAGATION_TIMEOUT_SECONDS / 60)) minutes for it to apply)"
expect_allowed_once_applied "create table" az storage table create -n "$ROLE_CHECK_TABLE" "${TABLE_ARGUMENTS[@]}"
expect_allowed_once_applied "insert row" az storage entity insert -t "$ROLE_CHECK_TABLE" \
  -e PartitionKey=check "RowKey=$RUN_ID" Value=first "${TABLE_ARGUMENTS[@]}"
expect forbidden "read rows" az storage entity query -t "$ROLE_CHECK_TABLE" "${TABLE_ARGUMENTS[@]}"
expect forbidden "merge row" az storage entity merge -t "$ROLE_CHECK_TABLE" \
  -e PartitionKey=check "RowKey=$RUN_ID" Value=changed "${TABLE_ARGUMENTS[@]}"
expect forbidden "replace row" az storage entity replace -t "$ROLE_CHECK_TABLE" \
  -e PartitionKey=check "RowKey=$RUN_ID" Value=changed "${TABLE_ARGUMENTS[@]}"
expect forbidden "upsert row" az storage entity insert -t "$ROLE_CHECK_TABLE" \
  -e PartitionKey=check "RowKey=$RUN_ID" Value=changed --if-exists replace "${TABLE_ARGUMENTS[@]}"
expect forbidden "delete row" az storage entity delete -t "$ROLE_CHECK_TABLE" \
  --partition-key check --row-key "$RUN_ID" "${TABLE_ARGUMENTS[@]}"
expect forbidden "list tables" az storage table list "${TABLE_ARGUMENTS[@]}"
expect forbidden "delete table" az storage table delete -n "$ROLE_CHECK_TABLE" --fail-not-exist "${TABLE_ARGUMENTS[@]}"
expect_allowed_once_applied "create append blob" az rest --method put --url "$BLOB_URL" \
  "${BLOB_ARGUMENTS[@]}" "x-ms-blob-type=AppendBlob" "If-None-Match=*"
expect allowed "append to blob" az rest --method put --url "$BLOB_URL?comp=appendblock" --body "first line" "${BLOB_ARGUMENTS[@]}"
expect forbidden "overwrite blob" az rest --method put --url "$BLOB_URL" \
  "${BLOB_ARGUMENTS[@]}" "x-ms-blob-type=AppendBlob"
expect forbidden "read blob" az rest --method get --url "$BLOB_URL" "${BLOB_ARGUMENTS[@]}"
expect forbidden "delete blob" az rest --method delete --url "$BLOB_URL" "${BLOB_ARGUMENTS[@]}"

cleanup
trap - EXIT
[ "$failures" -eq 0 ] || fail "$failures check(s) failed. $WRITER_ROLE was not given to the API"

step "Giving $WRITER_ROLE to the API ($CONTAINER_APP)"
api_principal_id="$(az containerapp show -n "$CONTAINER_APP" -g "$RESOURCE_GROUP" --query identity.principalId -o tsv)"
[ -n "$api_principal_id" ] || fail "$CONTAINER_APP has no system-assigned managed identity"
assign_role "$api_principal_id" ServicePrincipal "$WRITER_ROLE"

step "Reader roles for the signed-in owner"
owner_object_id="$(az ad signed-in-user show --query id -o tsv)" || fail "Run this script signed in as a user"
assign_role "$owner_object_id" User "$TABLE_READER_ROLE"
assign_role "$owner_object_id" User "$BLOB_READER_ROLE"

step "Role assignments on $AUDIT_ACCOUNT (expected: $WRITER_ROLE for the API, the two readers for you, and inherited Owner and Contributor)"
az role assignment list --scope "$AUDIT_ACCOUNT_ID" --include-inherited \
  --query "[].{principal:principalName, type:principalType, role:roleDefinitionName}" -o table

step "Done. Log: $LOG_FILE"
