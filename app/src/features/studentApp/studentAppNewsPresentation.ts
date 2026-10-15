import { firstNameOf, shortDayLabel } from "@/features/studentApp/studentAppSchedule";
import { StudentAppNewsItem } from "@/features/studentApp/types";
import { addDays, formatClockTime, toIsoDate, todayIsoDate } from "@/features/sessions/dates";
import {
  announcementCategoryPresentations,
  hasCategoryPill,
} from "@/features/announcements/announcementCategories";
import { translate, translateCount, TranslationKey } from "@/i18n/translate";
import { ThemeColorToken } from "@/theme/themeColorTokens";
import { IconName } from "@/ui/Icon";
import { StatusTone } from "@/ui/StatusPill";

export type NewsGroup = "today" | "week" | "earlier";

export const newsGroups: readonly NewsGroup[] = ["today", "week", "earlier"];

export const newsGroupLabels: Record<NewsGroup, TranslationKey> = {
  today: "student.news.group.today",
  week: "student.news.group.week",
  earlier: "student.news.group.earlier",
};

const daysPerWeek = 7;

export function localDateOf(isoDateTime: string): string {
  return toIsoDate(new Date(isoDateTime));
}

export function newsGroupOf(item: StudentAppNewsItem, today: string = todayIsoDate()): NewsGroup {
  const date = localDateOf(item.occurredAt);
  if (date === today) {
    return "today";
  }
  return date > addDays(today, -daysPerWeek) ? "week" : "earlier";
}

export function newsWhenLabel(item: StudentAppNewsItem, today: string = todayIsoDate()): string {
  const date = localDateOf(item.occurredAt);
  if (date === today) {
    return formatClockTime(new Date(item.occurredAt));
  }
  return date === addDays(today, -1) ? translate("student.news.yesterday") : shortDayLabel(date);
}

export interface NewsCategoryPill {
  label: string;
  tone: StatusTone;
}

export interface NewsPresentation {
  icon: IconName;
  title: string;
  body: string | null;
  iconTone?: ThemeColorToken;
  categoryPill?: NewsCategoryPill;
  audience?: string;
}

export function joinNames(names: readonly string[]): string {
  if (names.length <= 1) {
    return names[0] ?? "";
  }
  return translate("student.news.nameList", {
    first: names.slice(0, -1).join(", "),
    last: names[names.length - 1] ?? "",
  });
}

export function presentNews(item: StudentAppNewsItem): NewsPresentation {
  const student = joinNames(item.studentFullNames.map(firstNameOf));
  switch (item.kind) {
    case "OrderReady":
      return {
        icon: "delivered",
        title: translate("student.news.orderReady.title"),
        body: translate("student.news.orderReady.body"),
      };
    case "InstructorFeedback":
      return {
        icon: "comment",
        title:
          item.instructorFullName === null
            ? translate("student.news.feedback.titleWithoutInstructor", { student })
            : translate("student.news.feedback.title", {
                instructor: firstNameOf(item.instructorFullName),
                student,
              }),
        body: item.body === null ? null : `“${item.body}”`,
      };
    case "ClassCancelled":
      return {
        icon: "cancelled",
        title: translate("student.news.cancelled.title", {
          student,
          date: item.classDate === null ? "" : shortDayLabel(item.classDate),
        }),
        body: [item.className, item.body].filter(Boolean).join(" · ") || null,
      };
    case "ClassChanged":
      return {
        icon: "schedule",
        title: translate("student.news.changed.title", {
          student,
          date: item.classDate === null ? "" : shortDayLabel(item.classDate),
        }),
        body:
          [
            item.className,
            item.classStartTime === null
              ? null
              : translate("student.news.changed.time", { time: item.classStartTime }),
            item.instructorFullName === null
              ? null
              : translate("student.news.changed.instructor", {
                  instructor: firstNameOf(item.instructorFullName),
                }),
          ]
            .filter(Boolean)
            .join(" · ") || null,
      };
    default:
      return presentAnnouncement(item);
  }
}

function presentAnnouncement(item: StudentAppNewsItem): NewsPresentation {
  const category = item.category === null ? null : announcementCategoryPresentations[item.category];
  return {
    icon: category?.icon ?? "announcement",
    iconTone: category?.tone,
    title: item.title ?? "",
    body: item.body,
    categoryPill:
      item.category === null || category === null || !hasCategoryPill(item.category)
        ? undefined
        : { label: translate(category.labelKey), tone: category.tone },
    audience:
      item.audienceClassNames.length === 0
        ? undefined
        : translateCount("student.news.audience", item.audienceClassNames.length, {
            classes: joinNames(item.audienceClassNames),
          }),
  };
}

export function isPinnedAnnouncement(item: StudentAppNewsItem): boolean {
  return item.kind === "Announcement" && item.isPinned;
}
