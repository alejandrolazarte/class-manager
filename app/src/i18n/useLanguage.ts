import { useContext } from "react";
import { LanguageContext, LanguageContextValue } from "@/i18n/LanguageContext";

export function useLanguage(): LanguageContextValue {
  const languageContext = useContext(LanguageContext);
  if (languageContext === null) {
    throw new Error("useLanguage must be used inside LanguageProvider");
  }
  return languageContext;
}
