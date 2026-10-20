import { createRoutesAndBundles } from "../createRoutesAndBundles";

const { bundleSeparationViolations } = jest.requireActual("../../checkBundleSeparation.js");

const appRouteFiles = ["_layout.tsx", "(tabs)/today.tsx"];
const backofficeRouteFiles = ["_layout.tsx", "sign-in.tsx"];

describe("When each bundle has only its own routes", () => {
  it("Then no violation is reported", () => {
    const routesAndBundles = createRoutesAndBundles({
      appRouteFiles,
      appBundleRoutes: appRouteFiles,
      backofficeRouteFiles,
      backofficeBundleRoutes: backofficeRouteFiles,
    });

    expect(bundleSeparationViolations(routesAndBundles)).toEqual([]);
  });
});
