import { createWebExport } from "./createWebExport";

const bundlePath = "_expo/static/js/web/entry.js";

export interface RoutesAndBundles {
  appRoutes: string;
  appExport: string;
  backofficeRoutes: string;
  backofficeExport: string;
}

function routeKeysIn(routeFiles: string[]): string {
  return routeFiles.map((routeFile) => `"./${routeFile}"`).join(",");
}

export function createRoutesAndBundles({
  appRouteFiles,
  appBundleRoutes,
  backofficeRouteFiles,
  backofficeBundleRoutes,
}: {
  appRouteFiles: string[];
  appBundleRoutes: string[];
  backofficeRouteFiles: string[];
  backofficeBundleRoutes: string[];
}): RoutesAndBundles {
  return {
    appRoutes: createWebExport(Object.fromEntries(appRouteFiles.map((file) => [file, ""]))),
    appExport: createWebExport({ [bundlePath]: routeKeysIn(appBundleRoutes) }),
    backofficeRoutes: createWebExport(
      Object.fromEntries(backofficeRouteFiles.map((file) => [file, ""])),
    ),
    backofficeExport: createWebExport({ [bundlePath]: routeKeysIn(backofficeBundleRoutes) }),
  };
}
