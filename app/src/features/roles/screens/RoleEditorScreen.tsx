import { useRouter } from "expo-router";
import { useState } from "react";
import { isApiError } from "@/api/httpClient";
import { useCurrentMember } from "@/features/members/CurrentMemberProvider";
import { brandOnlyPermissions, Permission } from "@/features/members/permissions";
import { PermissionPicker } from "@/features/roles/components/PermissionPicker";
import { roleErrorCodes } from "@/features/roles/roleErrorCodes";
import { isSystemRole, roleKeyOfRole, roleName } from "@/features/roles/roleChoices";
import { CreateRoleRequest, Role } from "@/features/roles/types";
import { useCreateRole, useDeleteRole, useUpdateRole } from "@/features/roles/useRoleMutations";
import { useRoles } from "@/features/roles/useRoles";
import { SettingsFormScreenLayout } from "@/features/settings/components/SettingsFormScreenLayout";
import { SettingsItemState } from "@/features/settings/components/SettingsItemState";
import { SubmissionFailure, toSubmissionFailure } from "@/features/settings/submissionFailure";
import { translate } from "@/i18n/translate";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { TextField } from "@/ui/TextField";
import { useToast } from "@/ui/ToastProvider";
import { isFilled } from "@/forms/requiredFields";
import { RequiredFieldsLegend } from "@/ui/RequiredFieldsLegend";

const nameMinLength = 2;
const nameMaxLength = 60;

type RoleConflict = "inUse" | "instructorRequired" | "ownRole" | "exceedsOwn";

const conflictMessages = {
  inUse: "roles.editor.inUse",
  instructorRequired: "roles.editor.instructorRequired",
  ownRole: "roles.editor.ownRole",
  exceedsOwn: "roles.editor.exceedsOwn",
} as const;

interface RoleEditorProps {
  role?: Role;
  copiedFrom?: Role;
}

function conflictOf(saveError: unknown): RoleConflict | null {
  if (!isApiError(saveError)) {
    return null;
  }
  if (saveError.hasCode(roleErrorCodes.inUse)) {
    return "inUse";
  }
  if (saveError.hasCode(roleErrorCodes.instructorRequired)) {
    return "instructorRequired";
  }
  if (saveError.hasCode(roleErrorCodes.ownRole)) {
    return "ownRole";
  }
  return saveError.hasCode(roleErrorCodes.exceedsOwn) ? "exceedsOwn" : null;
}

function RoleEditor({ role, copiedFrom }: RoleEditorProps) {
  const router = useRouter();
  const { showToast } = useToast();
  const { permissions: memberPermissions } = useCurrentMember();
  const createMutation = useCreateRole();
  const updateMutation = useUpdateRole(role?.id ?? "");
  const deleteMutation = useDeleteRole();
  const [name, setName] = useState(
    role?.name ??
      (copiedFrom ? translate("roles.editor.copyName", { name: roleName(copiedFrom) }) : ""),
  );
  const [selected, setSelected] = useState<Permission[]>(
    (role ?? copiedFrom)?.permissions.filter((permission) =>
      memberPermissions.includes(permission),
    ) ?? [],
  );
  const [nameError, setNameError] = useState<string | undefined>();
  const [permissionsError, setPermissionsError] = useState<string | undefined>();
  const [conflict, setConflict] = useState<RoleConflict | null>(null);
  const [submissionFailure, setSubmissionFailure] = useState<SubmissionFailure | null>(null);
  const grantable = memberPermissions.filter(
    (permission) => !brandOnlyPermissions.includes(permission),
  );

  const save = async () => {
    setSubmissionFailure(null);
    setConflict(null);
    const trimmedName = name.trim();
    const isNameValid = trimmedName.length >= nameMinLength && trimmedName.length <= nameMaxLength;
    setNameError(isNameValid ? undefined : translate("roles.editor.nameInvalid"));
    setPermissionsError(
      selected.length > 0 ? undefined : translate("roles.editor.permissionsRequired"),
    );
    if (!isNameValid || selected.length === 0) {
      return;
    }
    try {
      if (role?.id) {
        await updateMutation.mutateAsync({ name: trimmedName, permissions: selected });
      } else {
        await createMutation.mutateAsync({
          name: trimmedName,
          permissions: selected,
          copiedFrom: copiedFrom ? (copiedFrom.systemRole ?? copiedFrom.name) : null,
        });
      }
      showToast(translate("roles.editor.saved"));
      router.back();
    } catch (saveError) {
      if (
        isApiError(saveError) &&
        (saveError.hasCode(roleErrorCodes.nameTaken) ||
          saveError.hasCode(roleErrorCodes.nameReserved))
      ) {
        setNameError(translate("roles.editor.nameTaken"));
        return;
      }
      const roleConflict = conflictOf(saveError);
      if (roleConflict) {
        setConflict(roleConflict);
        return;
      }
      setSubmissionFailure(toSubmissionFailure(saveError));
    }
  };

  const remove = async () => {
    if (!role?.id) {
      return;
    }
    setSubmissionFailure(null);
    setConflict(null);
    const deletedRole: CreateRoleRequest = {
      name: role.name ?? "",
      permissions: role.permissions,
      copiedFrom: role.copiedFrom,
    };
    try {
      await deleteMutation.mutateAsync(role.id);
      showToast(translate("roles.editor.deleted"), {
        label: translate("common.undo"),
        onPress: () => {
          createMutation
            .mutateAsync(deletedRole)
            .catch(() => showToast(translate("common.undoFailed")));
        },
      });
      router.back();
    } catch (deleteError) {
      const roleConflict = conflictOf(deleteError);
      if (roleConflict) {
        setConflict(roleConflict);
        return;
      }
      setSubmissionFailure(toSubmissionFailure(deleteError));
    }
  };

  return (
    <SettingsFormScreenLayout
      title={role ? roleName(role) : translate("roles.editor.newTitle")}
      submissionFailure={submissionFailure}
      onRetry={save}
    >
      {conflict ? <Banner tone="warning" message={translate(conflictMessages[conflict])} /> : null}
      <RequiredFieldsLegend />
      <TextField
        label={translate("roles.editor.name")}
        isRequired
        placeholder={translate("roles.editor.namePlaceholder")}
        value={name}
        onChangeText={setName}
        errorMessage={nameError}
      />
      <PermissionPicker selected={selected} grantable={grantable} onChange={setSelected} />
      {permissionsError ? <Banner tone="warning" message={permissionsError} /> : null}
      <Button
        label={translate("common.save")}
        onPress={save}
        disabled={!isFilled(name) || selected.length === 0}
        isLoading={createMutation.isPending || updateMutation.isPending}
      />
      {role?.id ? (
        <Button
          variant="dangerOutline"
          size="medium"
          label={translate("roles.editor.delete")}
          onPress={remove}
          isLoading={deleteMutation.isPending}
        />
      ) : null}
    </SettingsFormScreenLayout>
  );
}

interface RoleEditorScreenProps {
  roleKey?: string;
  copyFromRoleKey?: string;
}

export function RoleEditorScreen({ roleKey, copyFromRoleKey }: RoleEditorScreenProps) {
  const rolesQuery = useRoles();
  const findRole = (key: string | undefined) =>
    key === undefined ? undefined : rolesQuery.data?.find((role) => roleKeyOfRole(role) === key);
  const role = findRole(roleKey);
  const copiedFrom = findRole(copyFromRoleKey);
  const isWaitingForRole =
    (roleKey !== undefined && role === undefined) ||
    (copyFromRoleKey !== undefined && copiedFrom === undefined);
  if (isWaitingForRole || (roleKey !== undefined && isSystemRole(roleKey))) {
    return (
      <SettingsItemState
        navigation="close"
        isPending={rolesQuery.isPending}
        isError={rolesQuery.isError}
        notFoundMessage={translate("roles.notFound")}
        onRetry={() => rolesQuery.refetch()}
      />
    );
  }
  return (
    <RoleEditor key={roleKey ?? copyFromRoleKey ?? "new"} role={role} copiedFrom={copiedFrom} />
  );
}
