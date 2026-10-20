import { createRoutesAndBundles } from "../createRoutesAndBundles";

const { bundleSeparationViolations } = jest.requireActual("../../checkBundleSeparation.js");

const appRouteFiles = ["_layout.tsx", "(tabs)/today.tsx"];
const backofficeRouteFiles = ["_layout.tsx", "sign-in.tsx"];

describe("When a bundle has none of its own routes", () => {
  it("Then the check cannot pass", () => {
    const routesAndBundles = createRoutesAndBundles({
      appRouteFiles,
      appBundleRoutes: appRouteFiles,
      backofficeRouteFiles,
      backofficeBundleRoutes: [],
    });

    expect(bundleSeparationViolations(routesAndBundles)).toEqual([
      "The backoffice bundle contains none of its own routes, so the check proves nothing.",
    ]);
  });
});
