import { AppText, TextTone } from "@/ui/AppText";
import { Highlight, HighlightTone } from "@/ui/Highlight";

interface NoticeProps {
  message: string;
  tone: HighlightTone;
}

const textTones: Record<HighlightTone, TextTone> = {
  primary: "primarySoft",
  warning: "warningSoft",
  success: "successSoft",
  danger: "dangerSoft",
  muted: "muted",
};

export function Notice({ message, tone }: NoticeProps) {
  return (
    <Highlight tone={tone} isCompact>
      <AppText variant="caption" tone={textTones[tone]}>
        {message}
      </AppText>
    </Highlight>
  );
}
