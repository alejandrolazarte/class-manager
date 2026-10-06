import { TranslationKey } from "@/i18n/translate";
import { IconName } from "@/ui/Icon";

export interface StudentAppTabDefinition {
  name: string;
  titleKey: TranslationKey;
  icon: IconName;
}

export const studentAppTabDefinitions: readonly StudentAppTabDefinition[] = [
  { name: "index", titleKey: "student.tabs.home", icon: "home" },
  { name: "classes", titleKey: "student.tabs.classes", icon: "classes" },
  { name: "shop", titleKey: "student.tabs.shop", icon: "products" },
  { name: "progress", titleKey: "student.tabs.progress", icon: "achievements" },
  { name: "settings", titleKey: "settings.title", icon: "settings" },
];

export const studentAppShopTabName = "shop";

export const studentAppHiddenRouteNames: readonly string[] = ["orders", "news", "profile"];
