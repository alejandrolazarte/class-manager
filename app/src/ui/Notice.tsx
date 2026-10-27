import { AppText, TextTone } from "@/ui/AppText";
import { TonalBlock, TonalBlockTone } from "@/ui/TonalBlock";

interface NoticeProps {
  message: string;
  tone: TonalBlockTone;
}

const textTones: Record<TonalBlockTone, TextTone> = {
  primary: "primarySoft",
  warning: "warningSoft",
  success: "successSoft",
  danger: "dangerSoft",
  muted: "muted",
  surface: "default",
  inverse: "inverse",
};

export function Notice({ message, tone }: NoticeProps) {
  return (
    <TonalBlock tone={tone} isCompact>
      <AppText variant="caption" tone={textTones[tone]}>
        {message}
      </AppText>
    </TonalBlock>
  );
}
