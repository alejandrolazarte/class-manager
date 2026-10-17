import { SamePerson } from "@/features/users/samePerson";
import { TranslationKey, translate } from "@/i18n/translate";
import { ConfirmDialog } from "@/ui/ConfirmDialog";

interface SamePersonDialogProps {
  person: SamePerson;
  messageKey: TranslationKey;
  onCancel: () => void;
  onConfirm: () => void;
  isConfirming: boolean;
}

export function SamePersonDialog({
  person,
  messageKey,
  onCancel,
  onConfirm,
  isConfirming,
}: SamePersonDialogProps) {
  return (
    <ConfirmDialog
      title={translate("people.samePerson.question", { name: person.fullName })}
      message={translate(messageKey, { name: person.fullName, email: person.email })}
      confirmLabel={translate("people.samePerson.confirm")}
      onCancel={onCancel}
      onConfirm={onConfirm}
      isConfirming={isConfirming}
    />
  );
}
