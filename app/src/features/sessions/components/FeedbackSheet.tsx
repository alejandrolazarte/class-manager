import { useState } from "react";
import { View } from "react-native";
import { SessionStudent } from "@/features/sessions/types";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { BottomSheet } from "@/ui/BottomSheet";
import { Button } from "@/ui/Button";
import { TextField } from "@/ui/TextField";

const feedbackMaxLength = 500;

interface FeedbackSheetProps {
  student: SessionStudent;
  isSaving: boolean;
  onSave: (text: string | null) => void;
  onClose: () => void;
}

export function FeedbackSheet({ student, isSaving, onSave, onClose }: FeedbackSheetProps) {
  const [text, setText] = useState(student.feedback ?? "");
  return (
    <BottomSheet onClose={onClose}>
      <View className="gap-1">
        <AppText variant="headline" accessibilityRole="header">
          {translate("sessions.feedback.title", { name: student.studentFullName })}
        </AppText>
        <AppText variant="caption" tone="subtle">
          {translate("sessions.feedback.hint")}
        </AppText>
      </View>
      <TextField
        label={translate("sessions.feedback.label")}
        isRequired
        placeholder={translate("sessions.feedback.placeholder")}
        value={text}
        onChangeText={setText}
        maxLength={feedbackMaxLength}
        multiline
        autoFocus
      />
      <Button
        label={translate("sessions.feedback.save")}
        disabled={text.trim().length === 0}
        isLoading={isSaving}
        onPress={() => onSave(text)}
      />
      {student.feedback === null ? null : (
        <Button
          variant="dangerOutline"
          size="medium"
          label={translate("sessions.feedback.remove")}
          disabled={isSaving}
          onPress={() => onSave(null)}
        />
      )}
    </BottomSheet>
  );
}
