const { cpSync, readFileSync, rmSync, writeFileSync } = require("node:fs");
const { join } = require("node:path");

const manifestFileName = "manifest.webmanifest";
const iconsDirectoryName = "icons";
const indexFileName = "index.html";
const defaultSourceDirectory = "backoffice-web";

function metaTagPattern(name) {
  return new RegExp(`<meta name="${name}" content="[^"]*" />`);
}

function replaceMetaTag(indexHtml, name, content) {
  const pattern = metaTagPattern(name);
  if (!pattern.test(indexHtml)) {
    throw new Error(`The exported ${indexFileName} has no ${name} meta tag to replace.`);
  }
  return indexHtml.replace(pattern, `<meta name="${name}" content="${content}" />`);
}

function applyBackofficeWebIdentity(outputDirectory, sourceDirectory = defaultSourceDirectory) {
  const manifestPath = join(sourceDirectory, manifestFileName);
  const manifest = JSON.parse(readFileSync(manifestPath, "utf8"));
  const indexPath = join(outputDirectory, indexFileName);
  let indexHtml = readFileSync(indexPath, "utf8");
  indexHtml = replaceMetaTag(indexHtml, "theme-color", manifest.theme_color);
  indexHtml = replaceMetaTag(indexHtml, "apple-mobile-web-app-title", manifest.name);
  indexHtml = replaceMetaTag(indexHtml, "description", manifest.description);
  writeFileSync(indexPath, indexHtml);

  cpSync(manifestPath, join(outputDirectory, manifestFileName));
  rmSync(join(outputDirectory, iconsDirectoryName), { recursive: true, force: true });
  cpSync(join(sourceDirectory, iconsDirectoryName), join(outputDirectory, iconsDirectoryName), {
    recursive: true,
  });
}

module.exports = { applyBackofficeWebIdentity };
