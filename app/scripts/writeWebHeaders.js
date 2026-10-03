const { writeFileSync } = require("node:fs");
const { join } = require("node:path");

const headersFileName = "_headers";
const everyPathPattern = "/*";
const reportOnlyPolicyHeader = "Content-Security-Policy-Report-Only";
const defaultOutputDirectory = "dist";
const defaultApiBaseUrl = "http://localhost:5000";

function contentSecurityPolicy(apiBaseUrl) {
  const apiOrigin = new URL(apiBaseUrl).origin;
  const directives = [
    "default-src 'self'",
    "script-src 'self'",
    "style-src 'self' 'unsafe-inline'",
    "img-src 'self' data: blob:",
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

function writeWebHeaders(outputDirectory, apiBaseUrl) {
  const headers = `${everyPathPattern}\n  ${reportOnlyPolicyHeader}: ${contentSecurityPolicy(apiBaseUrl)}\n`;
  writeFileSync(join(outputDirectory, headersFileName), headers);
}

if (require.main === module) {
  const outputDirectory = process.argv[2] ?? defaultOutputDirectory;
  const apiBaseUrl = process.env.EXPO_PUBLIC_API_BASE_URL ?? defaultApiBaseUrl;
  writeWebHeaders(outputDirectory, apiBaseUrl);
  console.log(
    `Wrote ${headersFileName} with a report-only content security policy in ${outputDirectory}`,
  );
}

module.exports = { contentSecurityPolicy, writeWebHeaders };
