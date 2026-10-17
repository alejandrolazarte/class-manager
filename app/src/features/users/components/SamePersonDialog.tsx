import { PersonRole, SamePersonQuestion } from "@/features/users/samePersonQuestion";
import { TranslationKey, translate } from "@/i18n/translate";
import { ConfirmDialog } from "@/ui/ConfirmDialog";

const roleLabels: Record<PersonRole, TranslationKey> = {
  Client: "people.role.client",
  Student: "people.role.student",
  Instructor: "people.role.instructor",
  Team: "people.role.team",
};

interface SamePersonDialogProps {
  question: SamePersonQuestion;
  onCancel: () => void;
  onConfirm: () => void;
  isConfirming: boolean;
}

export function SamePersonDialog({
  question,
  onCancel,
  onConfirm,
  isConfirming,
}: SamePersonDialogProps) {
  const roles = question.roles.map((role) => translate(roleLabels[role])).join(", ");
  return (
    <ConfirmDialog
      title={translate("people.samePerson.title", { name: question.fullName })}
      message={
        roles.length > 0
          ? translate("people.samePerson.message", { name: question.fullName, roles })
          : translate("people.samePerson.messageWithoutRoles", { name: question.fullName })
      }
      confirmLabel={translate("people.samePerson.confirm")}
      onCancel={onCancel}
      onConfirm={onConfirm}
      isConfirming={isConfirming}
    />
  );
}
