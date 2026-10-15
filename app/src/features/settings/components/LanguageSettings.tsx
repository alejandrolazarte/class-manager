import { useMutation, useQueryClient } from "@tanstack/react-query";
import { View } from "react-native";
import { updateMyLanguage } from "@/features/account/accountApi";
import { accountQueryKeys } from "@/features/account/accountQueryKeys";
import { languageValue } from "@/features/settings/components/languageValue";
import { isLanguage, Language, languages } from "@/i18n/languages";
import { translate } from "@/i18n/translate";
import { useLanguage } from "@/i18n/useLanguage";
import { AppText } from "@/ui/AppText";
import { SegmentedControl } from "@/ui/SegmentedControl";
import { useToast } from "@/ui/ToastProvider";

export function LanguageSettings() {
  const { language, setLanguage } = useLanguage();
  const queryClient = useQueryClient();
  const { showToast } = useToast();
  const languageMutation = useMutation({
    mutationFn: updateMyLanguage,
    onSuccess: (updatedAccount) => {
      queryClient.setQueryData(accountQueryKeys.myAccount, updatedAccount);
      if (isLanguage(updatedAccount.preferredLanguage)) {
        setLanguage(updatedAccount.preferredLanguage);
      }
    },
    onError: () => showToast(translate("common.unexpectedError")),
  });
  const shownLanguage = languageMutation.isPending ? languageMutation.variables : language;

  const chooseLanguage = (chosenLanguage: Language) => {
    if (chosenLanguage !== language) {
      languageMutation.mutate(chosenLanguage);
    }
  };

  return (
    <View className="gap-3">
      <AppText variant="headline" accessibilityRole="header">
        {translate("settings.language")}
      </AppText>
      <SegmentedControl
        options={languages.map((optionLanguage) => ({
          value: optionLanguage,
          label: languageValue(optionLanguage),
        }))}
        selectedValue={shownLanguage}
        onChange={chooseLanguage}
      />
      <AppText variant="caption" tone="muted">
        {translate("settings.language.hint")}
      </AppText>
    </View>
  );
}
