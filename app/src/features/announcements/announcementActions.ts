import { AnnouncementAction } from "@/features/announcements/types";
import { TranslationKey } from "@/i18n/translate";

export const announcementActions: readonly AnnouncementAction[] = [
  "None",
  "Shop",
  "Classes",
  "Link",
];

export const announcementActionOptionKeys: Record<AnnouncementAction, TranslationKey> = {
  None: "announcements.action.none",
  Shop: "announcements.action.shop",
  Classes: "announcements.action.classes",
  Link: "announcements.action.link",
};

export const announcementActionButtonKeys: Record<
  Exclude<AnnouncementAction, "None">,
  TranslationKey
> = {
  Shop: "student.news.action.shop",
  Classes: "student.news.action.classes",
  Link: "student.news.action.link",
};

const httpsLinkPattern = /^https:\/\/\S+$/i;

export const announcementActionUrlMaxLength = 500;

export function isHttpsLink(link: string): boolean {
  return link.length <= announcementActionUrlMaxLength && httpsLinkPattern.test(link);
}
