import { GestureResponderEvent } from "react-native";
import { AppText } from "@/ui/AppText";

interface InlineLinkProps {
  label: string;
  onPress: () => void;
}

export function InlineLink({ label, onPress }: InlineLinkProps) {
  const openLink = (event: GestureResponderEvent) => {
    event.stopPropagation();
    onPress();
  };
  return (
    <AppText variant="bodyStrong" tone="primary" accessibilityRole="link" onPress={openLink}>
      {label}
    </AppText>
  );
}
