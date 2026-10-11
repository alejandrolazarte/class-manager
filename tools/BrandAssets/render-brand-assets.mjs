import { readFileSync } from "node:fs";
import { createRequire } from "node:module";
import { dirname, join } from "node:path";
import { fileURLToPath } from "node:url";

const repositoryRoot = join(dirname(fileURLToPath(import.meta.url)), "..", "..");
const requireFromE2e = createRequire(join(repositoryRoot, "e2e", "package.json"));
const { chromium } = requireFromE2e("@playwright/test");

const brandColor = "#6d28d9";
const markSvg = readFileSync(join(repositoryRoot, "app", "assets", "brand", "laztty-mark.svg"), "utf8");
const appImagesFolder = join(repositoryRoot, "app", "assets", "images");
const webIconsFolder = join(repositoryRoot, "app", "public", "icons");

const fullBleed = { background: brandColor, cornerRatio: 0 };
const roundedTile = { background: brandColor, cornerRatio: 0.32 };
const transparent = { background: "transparent", cornerRatio: 0 };

const outputs = [
  { path: join(appImagesFolder, "icon.png"), size: 1024, markRatio: 0.58, ...fullBleed },
  { path: join(appImagesFolder, "android-icon-background.png"), size: 512, markRatio: 0, ...fullBleed },
  { path: join(appImagesFolder, "android-icon-foreground.png"), size: 512, markRatio: 0.4, ...transparent },
  { path: join(appImagesFolder, "android-icon-monochrome.png"), size: 432, markRatio: 0.4, ...transparent },
  { path: join(appImagesFolder, "splash-icon.png"), size: 512, markRatio: 0.58, ...roundedTile },
  { path: join(appImagesFolder, "favicon.png"), size: 48, markRatio: 0.62, ...roundedTile },
  { path: join(webIconsFolder, "icon-192.png"), size: 192, markRatio: 0.58, ...roundedTile },
  { path: join(webIconsFolder, "icon-512.png"), size: 512, markRatio: 0.58, ...roundedTile },
  { path: join(webIconsFolder, "icon-maskable-512.png"), size: 512, markRatio: 0.46, ...fullBleed },
  { path: join(webIconsFolder, "apple-touch-icon.png"), size: 180, markRatio: 0.58, ...fullBleed },
];

function pageFor({ size, markRatio, background, cornerRatio }) {
  const markSize = Math.round(size * markRatio);
  return `<!doctype html><html><body style="margin:0;background:transparent">
    <div style="width:${size}px;height:${size}px;display:grid;place-items:center;background:${background};border-radius:${size * cornerRatio}px">
      <div style="width:${markSize}px;height:${markSize}px">${markSvg.replace("<svg ", '<svg width="100%" height="100%" ')}</div>
    </div></body></html>`;
}

const browser = await chromium.launch({ executablePath: process.env.CHROMIUM_PATH });
const page = await browser.newPage();
for (const output of outputs) {
  await page.setViewportSize({ width: output.size, height: output.size });
  await page.setContent(pageFor(output));
  await page.screenshot({ path: output.path, omitBackground: true });
  console.log(`${output.path} (${output.size}px)`);
}
await browser.close();
