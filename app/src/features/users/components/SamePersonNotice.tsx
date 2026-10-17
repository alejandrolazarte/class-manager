import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Button } from "@/ui/Button";
import { TonalBlock } from "@/ui/TonalBlock";

interface SamePersonNoticeProps {
  message: string;
  onSeparate: () => void;
}

export function SamePersonNotice({ message, onSeparate }: SamePersonNoticeProps) {
  return (
    <TonalBlock tone="muted" isCompact>
      <AppText variant="body">{message}</AppText>
      <AppText variant="footnote" tone="subtle">
        {translate("people.separate.noticeHint")}
      </AppText>
      <Button
        variant="dangerOutline"
        size="small"
        label={translate("people.separate.open")}
        onPress={onSeparate}
      />
    </TonalBlock>
  );
}
