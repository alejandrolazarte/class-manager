import * as Sentry from "@sentry/react-native";
import { appConfiguration } from "@/config/appConfiguration";
import {
  keepBreadcrumbWithoutPersonalData,
  removePersonalData,
} from "@/errorReporting/errorReportPrivacy";

export function startErrorReporting(): void {
  const dsn = appConfiguration.errorReportingDsn;
  if (!dsn) {
    return;
  }
  Sentry.init({
    dsn,
    release: appConfiguration.errorReportingRelease,
    sendDefaultPii: false,
    beforeSend: removePersonalData,
    beforeBreadcrumb: keepBreadcrumbWithoutPersonalData,
  });
}
