const { writeFileSync } = require("node:fs");
const { join } = require("node:path");

const headersFileName = "_headers";
const everyPathPattern = "/*";
const reportOnlyPolicyHeader = "Content-Security-Policy-Report-Only";
const defaultOutputDirectory = "dist";
const defaultApiBaseUrl = "http://localhost:5000";

function contentSecurityPolicy(apiBaseUrl, filesBaseUrl) {
  const apiOrigin = new URL(apiBaseUrl).origin;
  const imageSources = ["'self'", "data:", "blob:"];
  if (filesBaseUrl) {
    imageSources.push(new URL(filesBaseUrl).origin);
  }
  const directives = [
    "default-src 'self'",
    "script-src 'self'",
    "style-src 'self' 'unsafe-inline'",
    `img-src ${imageSources.join(" ")}`,
    "font-src 'self' data:",
    `connect-src 'self' ${apiOrigin}`,
    "worker-src 'self'",
    "manifest-src 'self'",
    "object-src 'none'",
    "base-uri 'self'",
    "form-action 'self'",
    "frame-ancestors 'none'",
  ];
  return `${directives.join("; ")};`;
}

function writeWebHeaders(outputDirectory, apiBaseUrl, filesBaseUrl) {
  const headers = `${everyPathPattern}\n  ${reportOnlyPolicyHeader}: ${contentSecurityPolicy(apiBaseUrl, filesBaseUrl)}\n`;
  writeFileSync(join(outputDirectory, headersFileName), headers);
}

if (require.main === module) {
  const outputDirectory = process.argv[2] ?? defaultOutputDirectory;
  const apiBaseUrl = process.env.EXPO_PUBLIC_API_BASE_URL ?? defaultApiBaseUrl;
  writeWebHeaders(outputDirectory, apiBaseUrl, process.env.FILES_BASE_URL);
  console.log(
    `Wrote ${headersFileName} with a report-only content security policy in ${outputDirectory}`,
  );
}

module.exports = { contentSecurityPolicy, writeWebHeaders };
