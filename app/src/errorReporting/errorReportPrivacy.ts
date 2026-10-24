import type { Breadcrumb, ErrorEvent } from "@sentry/react-native";

const queryStringSeparator = "?";
const breadcrumbCategoriesWithPersonalData = ["console", "touch", "ui."];
const breadcrumbAddressFields = ["url", "from", "to"];

function withoutQueryString(address: string): string {
  const separatorIndex = address.indexOf(queryStringSeparator);
  return separatorIndex < 0 ? address : address.slice(0, separatorIndex);
}

export function removePersonalData(errorReport: ErrorEvent): ErrorEvent {
  const url = errorReport.request?.url;
  const request = url === undefined ? {} : { url: withoutQueryString(url) };
  return { ...errorReport, user: undefined, request: errorReport.request && request };
}

export function keepBreadcrumbWithoutPersonalData(breadcrumb: Breadcrumb): Breadcrumb | null {
  const category = breadcrumb.category ?? "";
  if (breadcrumbCategoriesWithPersonalData.some((prefix) => category.startsWith(prefix))) {
    return null;
  }
  if (!breadcrumb.data) {
    return breadcrumb;
  }
  const data = Object.fromEntries(
    Object.entries(breadcrumb.data).map(([field, value]) =>
      breadcrumbAddressFields.includes(field) && typeof value === "string"
        ? [field, withoutQueryString(value)]
        : [field, value],
    ),
  );
  return { ...breadcrumb, data };
}
