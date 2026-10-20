import { createRoutesAndBundles } from "../createRoutesAndBundles";

const { bundleSeparationViolations } = jest.requireActual("../../checkBundleSeparation.js");

const appRouteFiles = ["_layout.tsx", "(tabs)/today.tsx"];
const backofficeRouteFiles = ["_layout.tsx", "sign-in.tsx"];

describe("When the backoffice bundle has an app route", () => {
  it("Then it is reported", () => {
    const routesAndBundles = createRoutesAndBundles({
      appRouteFiles,
      appBundleRoutes: appRouteFiles,
      backofficeRouteFiles,
      backofficeBundleRoutes: [...backofficeRouteFiles, "(tabs)/today.tsx"],
    });

    expect(bundleSeparationViolations(routesAndBundles)).toEqual([
      "The backoffice bundle contains the app route ./(tabs)/today.tsx.",
    ]);
  });
});
