const { writeFileSync } = require("node:fs");
const { join } = require("node:path");

const configFileName = "staticwebapp.config.json";
const reportOnlyPolicyHeader = "Content-Security-Policy-Report-Only";
const singlePageEntry = "/index.html";
const filesExcludedFromFallback = ["/_expo/*", "/assets/*", "/fonts/*", "/icons/*"];
const emailFontsRoute = "/fonts/*";
const emailFontsHeaders = {
  "Access-Control-Allow-Origin": "*",
  "Cache-Control": "public, max-age=31536000, immutable",
};
const webManifestMimeTypes = { ".webmanifest": "application/manifest+json" };
const defaultOutputDirectory = "dist";
const defaultApiBaseUrl = "http://localhost:5000";
const jsonIndentation = 2;

function contentSecurityPolicy(apiBaseUrl, filesBaseUrl, errorReportingDsn) {
  const imageSources = ["'self'", "data:", "blob:"];
  if (filesBaseUrl) {
    imageSources.push(new URL(filesBaseUrl).origin);
  }
  const connectSources = ["'self'", new URL(apiBaseUrl).origin];
  if (errorReportingDsn) {
    connectSources.push(new URL(errorReportingDsn).origin);
  }
  const directives = [
    "default-src 'self'",
    "script-src 'self'",
    "style-src 'self' 'unsafe-inline'",
    `img-src ${imageSources.join(" ")}`,
    "font-src 'self' data:",
    `connect-src ${connectSources.join(" ")}`,
    "worker-src 'self'",
    "manifest-src 'self'",
    "object-src 'none'",
    "base-uri 'self'",
    "form-action 'self'",
    "frame-ancestors 'none'",
  ];
  return `${directives.join("; ")};`;
}

function staticWebAppConfig(apiBaseUrl, filesBaseUrl, errorReportingDsn) {
  return {
    navigationFallback: { rewrite: singlePageEntry, exclude: filesExcludedFromFallback },
    globalHeaders: {
      [reportOnlyPolicyHeader]: contentSecurityPolicy(apiBaseUrl, filesBaseUrl, errorReportingDsn),
    },
    routes: [{ route: emailFontsRoute, headers: emailFontsHeaders }],
    mimeTypes: webManifestMimeTypes,
  };
}

function writeStaticWebAppConfig(outputDirectory, apiBaseUrl, filesBaseUrl, errorReportingDsn) {
  const config = staticWebAppConfig(apiBaseUrl, filesBaseUrl, errorReportingDsn);
  writeFileSync(
    join(outputDirectory, configFileName),
    `${JSON.stringify(config, null, jsonIndentation)}\n`,
  );
}

if (require.main === module) {
  const outputDirectory = process.argv[2] ?? defaultOutputDirectory;
  const apiBaseUrl = process.env.EXPO_PUBLIC_API_BASE_URL ?? defaultApiBaseUrl;
  writeStaticWebAppConfig(
    outputDirectory,
    apiBaseUrl,
    process.env.FILES_BASE_URL,
    process.env.EXPO_PUBLIC_SENTRY_DSN,
  );
  console.log(
    `Wrote ${configFileName} with a report-only content security policy in ${outputDirectory}`,
  );
}

module.exports = { contentSecurityPolicy, writeStaticWebAppConfig };
