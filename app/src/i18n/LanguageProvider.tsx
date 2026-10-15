import { Fragment, ReactNode, useCallback, useEffect, useMemo, useState } from "react";
import { Platform } from "react-native";
import { LanguageContext, LanguageContextValue } from "@/i18n/LanguageContext";
import { Language } from "@/i18n/languages";
import { parseLanguagePreference } from "@/i18n/languagePreference";
import { languagePreferenceStorage } from "@/i18n/languagePreferenceStorage";
import { setTranslationLanguage } from "@/i18n/translate";

interface LanguageProviderProps {
  children: ReactNode;
  initialLanguage?: Language;
}

function applyLanguage(language: Language): void {
  setTranslationLanguage(language);
  if (Platform.OS === "web") {
    globalThis.document?.documentElement.setAttribute("lang", language);
  }
}

export function LanguageProvider({ children, initialLanguage }: LanguageProviderProps) {
  const [language, setLanguageState] = useState<Language | null>(() => {
    if (initialLanguage === undefined) {
      return null;
    }
    applyLanguage(initialLanguage);
    return initialLanguage;
  });

  useEffect(() => {
    if (initialLanguage !== undefined) {
      return;
    }
    let isActive = true;
    languagePreferenceStorage
      .read()
      .catch(() => null)
      .then((storedLanguage) => {
        if (isActive) {
          const restoredLanguage = parseLanguagePreference(storedLanguage);
          applyLanguage(restoredLanguage);
          setLanguageState(restoredLanguage);
        }
      });
    return () => {
      isActive = false;
    };
  }, [initialLanguage]);

  const setLanguage = useCallback((chosenLanguage: Language) => {
    applyLanguage(chosenLanguage);
    setLanguageState(chosenLanguage);
    languagePreferenceStorage.write(chosenLanguage).catch(() => undefined);
  }, []);

  const languageContextValue = useMemo<LanguageContextValue | null>(
    () => (language === null ? null : { language, setLanguage }),
    [language, setLanguage],
  );

  if (languageContextValue === null) {
    return null;
  }

  return (
    <LanguageContext.Provider value={languageContextValue}>
      <Fragment key={languageContextValue.language}>{children}</Fragment>
    </LanguageContext.Provider>
  );
}
