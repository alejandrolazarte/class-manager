import { firstNameOf, shortDayLabel } from "@/features/family/familySchedule";
import { FamilyNewsItem } from "@/features/family/types";
import { addDays, toIsoDate, todayIsoDate } from "@/features/sessions/dates";
import { translate, TranslationKey } from "@/i18n/translate";
import { IconName } from "@/ui/Icon";

export type NewsGroup = "today" | "week" | "earlier";

export const newsGroups: readonly NewsGroup[] = ["today", "week", "earlier"];

export const newsGroupLabels: Record<NewsGroup, TranslationKey> = {
  today: "family.news.group.today",
  week: "family.news.group.week",
  earlier: "family.news.group.earlier",
};

const daysPerWeek = 7;

export function localDateOf(isoDateTime: string): string {
  return toIsoDate(new Date(isoDateTime));
}

export function newsGroupOf(item: FamilyNewsItem, today: string = todayIsoDate()): NewsGroup {
  const date = localDateOf(item.occurredAt);
  if (date === today) {
    return "today";
  }
  return date > addDays(today, -daysPerWeek) ? "week" : "earlier";
}

export function newsWhenLabel(item: FamilyNewsItem, today: string = todayIsoDate()): string {
  const date = localDateOf(item.occurredAt);
  if (date === today) {
    const occurredAt = new Date(item.occurredAt);
    return `${String(occurredAt.getHours()).padStart(2, "0")}:${String(occurredAt.getMinutes()).padStart(2, "0")}`;
  }
  return date === addDays(today, -1) ? translate("family.news.yesterday") : shortDayLabel(date);
}

export interface NewsPresentation {
  icon: IconName;
  title: string;
  body: string | null;
}

export function joinNames(names: readonly string[]): string {
  if (names.length <= 1) {
    return names[0] ?? "";
  }
  return translate("family.news.nameList", {
    first: names.slice(0, -1).join(", "),
    last: names[names.length - 1] ?? "",
  });
}

export function presentNews(item: FamilyNewsItem): NewsPresentation {
  const student = joinNames(item.studentFullNames.map(firstNameOf));
  switch (item.kind) {
    case "OrderReady":
      return {
        icon: "delivered",
        title: translate("family.news.orderReady.title"),
        body: translate("family.news.orderReady.body"),
      };
    case "CoachFeedback":
      return {
        icon: "comment",
        title:
          item.instructorFullName === null
            ? translate("family.news.feedback.titleWithoutInstructor", { student })
            : translate("family.news.feedback.title", {
                instructor: firstNameOf(item.instructorFullName),
                student,
              }),
        body: item.body === null ? null : `“${item.body}”`,
      };
    case "ClassCancelled":
      return {
        icon: "cancelled",
        title: translate("family.news.cancelled.title", {
          student,
          date: item.classDate === null ? "" : shortDayLabel(item.classDate),
        }),
        body: [item.className, item.body].filter(Boolean).join(" · ") || null,
      };
    default:
      return { icon: "announcement", title: item.title ?? "", body: item.body };
  }
}
