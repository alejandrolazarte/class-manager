import { View } from "react-native";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Checkbox } from "@/ui/Checkbox";

interface ContactAdultConfirmationProps {
  contactFullName: string;
  isConfirmed: boolean;
  onConfirmedChange: (isConfirmed: boolean) => void;
}

export function ContactAdultConfirmation({
  contactFullName,
  isConfirmed,
  onConfirmedChange,
}: ContactAdultConfirmationProps) {
  const name = contactFullName.trim();
  return (
    <View className="gap-1">
      <Checkbox
        label={
          name.length > 0
            ? translate("student.invite.confirmsAdult", { name })
            : translate("student.invite.confirmsAdultUnnamed")
        }
        isChecked={isConfirmed}
        onCheckedChange={onConfirmedChange}
        isRequired
      />
      <AppText variant="caption" tone="subtle">
        {translate("student.invite.confirmsAdultHint")}
      </AppText>
    </View>
  );
}
