import { AnnouncementCategory } from "@/features/announcements/types";
import { TranslationKey } from "@/i18n/translate";
import { IconName } from "@/ui/Icon";
import { StatusTone } from "@/ui/StatusPill";
import { TonalBlockTone } from "@/ui/TonalBlock";

type AnnouncementCategoryTone = StatusTone & TonalBlockTone;

interface AnnouncementCategoryPresentation {
  labelKey: TranslationKey;
  icon: IconName;
  tone: AnnouncementCategoryTone;
}

export const announcementCategories: readonly AnnouncementCategory[] = [
  "News",
  "Important",
  "Event",
  "Reminder",
];

export const defaultAnnouncementCategory: AnnouncementCategory = "News";

export function hasCategoryPill(category: AnnouncementCategory): boolean {
  return category !== defaultAnnouncementCategory;
}

export const announcementCategoryPresentations: Record<
  AnnouncementCategory,
  AnnouncementCategoryPresentation
> = {
  News: { labelKey: "announcements.category.news", icon: "announcement", tone: "primary" },
  Important: { labelKey: "announcements.category.important", icon: "warning", tone: "danger" },
  Event: { labelKey: "announcements.category.event", icon: "celebration", tone: "success" },
  Reminder: { labelKey: "announcements.category.reminder", icon: "notifications", tone: "warning" },
};
