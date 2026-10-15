import { useEffect } from "react";
import { useMyAccount } from "@/features/account/useMyAccount";
import { isLanguage } from "@/i18n/languages";
import { useLanguage } from "@/i18n/useLanguage";

export function AccountLanguageSync() {
  const { data: account } = useMyAccount();
  const { language, setLanguage } = useLanguage();
  const accountLanguage = account?.preferredLanguage ?? null;

  useEffect(() => {
    if (isLanguage(accountLanguage) && accountLanguage !== language) {
      setLanguage(accountLanguage);
    }
  }, [accountLanguage, language, setLanguage]);

  return null;
}
