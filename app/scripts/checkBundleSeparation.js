const { readdirSync, readFileSync, statSync } = require("node:fs");
const { join, relative } = require("node:path");

const bundleDirectory = join("_expo", "static", "js");
const bundleExtension = ".js";
const routeKeyPrefix = "./";
const defaultAppExport = "dist";
const defaultBackofficeExport = "dist-backoffice";
const appRoutesDirectory = "app";
const backofficeRoutesDirectory = "backoffice";

function filesUnder(directory) {
  return readdirSync(directory).flatMap((entry) => {
    const path = join(directory, entry);
    return statSync(path).isDirectory() ? filesUnder(path) : [path];
  });
}

function routeKeysOf(routesDirectory) {
  return filesUnder(routesDirectory).map(
    (file) => routeKeyPrefix + relative(routesDirectory, file).split("\\").join("/"),
  );
}

function bundleTextOf(exportDirectory) {
  return filesUnder(join(exportDirectory, bundleDirectory))
    .filter((file) => file.endsWith(bundleExtension))
    .map((file) => readFileSync(file, "utf8"))
    .join("\n");
}

function containsRoute(bundleText, routeKey) {
  return bundleText.includes(`"${routeKey}"`);
}

function violationsOfBundle(name, otherName, bundleText, ownRouteKeys, otherRouteKeys) {
  const violations = otherRouteKeys
    .filter((routeKey) => containsRoute(bundleText, routeKey))
    .map((routeKey) => `The ${name} bundle contains the ${otherName} route ${routeKey}.`);
  if (!ownRouteKeys.some((routeKey) => containsRoute(bundleText, routeKey))) {
    violations.push(
      `The ${name} bundle contains none of its own routes, so the check proves nothing.`,
    );
  }
  return violations;
}

function bundleSeparationViolations({ appRoutes, appExport, backofficeRoutes, backofficeExport }) {
  const appRouteKeys = routeKeysOf(appRoutes);
  const backofficeRouteKeys = routeKeysOf(backofficeRoutes);
  const onlyAppRouteKeys = appRouteKeys.filter((key) => !backofficeRouteKeys.includes(key));
  const onlyBackofficeRouteKeys = backofficeRouteKeys.filter((key) => !appRouteKeys.includes(key));
  return [
    ...violationsOfBundle(
      "app",
      "backoffice",
      bundleTextOf(appExport),
      onlyAppRouteKeys,
      onlyBackofficeRouteKeys,
    ),
    ...violationsOfBundle(
      "backoffice",
      "app",
      bundleTextOf(backofficeExport),
      onlyBackofficeRouteKeys,
      onlyAppRouteKeys,
    ),
  ];
}

if (require.main === module) {
  const violations = bundleSeparationViolations({
    appRoutes: appRoutesDirectory,
    appExport: process.argv[2] ?? defaultAppExport,
    backofficeRoutes: backofficeRoutesDirectory,
    backofficeExport: process.argv[3] ?? defaultBackofficeExport,
  });
  if (violations.length > 0) {
    console.error(violations.join("\n"));
    process.exit(1);
  }
  console.log("The app and backoffice bundles only contain their own routes.");
}

module.exports = { bundleSeparationViolations };
