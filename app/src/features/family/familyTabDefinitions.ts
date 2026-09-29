import { TranslationKey } from "@/i18n/translate";
import { IconName } from "@/ui/Icon";

export interface FamilyTabDefinition {
  name: string;
  titleKey: TranslationKey;
  icon: IconName;
}

export const familyTabDefinitions: readonly FamilyTabDefinition[] = [
  { name: "index", titleKey: "family.tabs.home", icon: "home" },
  { name: "classes", titleKey: "family.tabs.classes", icon: "classes" },
  { name: "shop", titleKey: "family.tabs.shop", icon: "products" },
];

export const familyShopTabName = "shop";

export const familyHiddenRouteNames: readonly string[] = ["orders"];
