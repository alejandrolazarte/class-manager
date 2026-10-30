import { ThemeColorToken } from "@/theme/themeColorTokens";
import { AppText, TextTone } from "@/ui/AppText";
import { Icon, IconName } from "@/ui/Icon";
import { PressableScale } from "@/ui/motion/PressableScale";

type Answer = "yes" | "no";

export type AnswerState = "open" | "chosen" | "notChosen";

interface AnswerButtonProps {
  label: string;
  answer: Answer;
  state: AnswerState;
  onPress: () => void;
  disabled?: boolean;
}

const answerIcons: Record<Answer, IconName> = {
  yes: "checked",
  no: "close",
};

const containerClassNames: Record<Answer, Record<AnswerState, string>> = {
  yes: {
    open: "border-transparent bg-success-soft",
    chosen: "border-transparent bg-success",
    notChosen: "border-border bg-transparent",
  },
  no: {
    open: "border-transparent bg-danger-soft",
    chosen: "border-transparent bg-danger",
    notChosen: "border-border bg-transparent",
  },
};

const textTones: Record<Answer, Record<AnswerState, TextTone>> = {
  yes: { open: "successSoft", chosen: "onSuccess", notChosen: "muted" },
  no: { open: "dangerSoft", chosen: "onDanger", notChosen: "muted" },
};

const iconTones: Record<Answer, Record<AnswerState, ThemeColorToken>> = {
  yes: {
    open: "success-soft-foreground",
    chosen: "success-foreground",
    notChosen: "muted-foreground",
  },
  no: {
    open: "danger-soft-foreground",
    chosen: "danger-foreground",
    notChosen: "muted-foreground",
  },
};

const disabledClassName = "border-border bg-surface";

const answerButtonHitSlop = { top: 4, bottom: 4, left: 2, right: 2 };

export function AnswerButton({
  label,
  answer,
  state,
  onPress,
  disabled = false,
}: AnswerButtonProps) {
  return (
    <PressableScale
      accessibilityRole="button"
      accessibilityLabel={label}
      accessibilityState={{ selected: state === "chosen", disabled }}
      disabled={disabled}
      onPress={onPress}
      hitSlop={answerButtonHitSlop}
      className={`min-h-9 flex-row items-center justify-center gap-1.5 rounded-full border px-3.5 ${disabled ? disabledClassName : containerClassNames[answer][state]}`}
    >
      <Icon
        name={answerIcons[answer]}
        size="small"
        tone={disabled ? "disabled-foreground" : iconTones[answer][state]}
      />
      <AppText variant="link" tone={disabled ? "disabled" : textTones[answer][state]}>
        {label}
      </AppText>
    </PressableScale>
  );
}
