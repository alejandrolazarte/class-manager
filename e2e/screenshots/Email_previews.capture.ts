import { Browser, test } from "@playwright/test";
import { spawnSync } from "node:child_process";
import { mkdirSync, readFileSync } from "node:fs";
import { basename, join, resolve } from "node:path";
import { pathToFileURL } from "node:url";

interface EmailPreviewFile {
  file: string;
  email: string;
  brand: string;
  subject: string;
}

const outputDirectory = process.env.SCREENSHOTS_DIR ?? "screenshots-output";
const blockedWebFont = "blocked";
const isWebFontBlocked = process.env.SCREENSHOTS_EMAIL_FONT === blockedWebFont;
const emailsDirectory = join(
  outputDirectory,
  isWebFontBlocked ? "correos-sin-fuente-web" : "correos",
);
const htmlDirectory = resolve(emailsDirectory, "html");
const emailPreviewsProject = "../tools/EmailPreviews/EmailPreviews.csproj";
const manifestFileName = "manifest.json";
const webFontRequests = /\.(woff2?|ttf|otf)(\?.*)?$/;
const webAppFontsDirectory = resolve("../app/public/fonts");
const widths: [folderName: string, width: number, height: number][] = [
  ["telefono", 390, 844],
  ["escritorio", 1280, 900],
];

test("Email previews", async ({ browser }) => {
  renderEmailPreviews();
  const previews: EmailPreviewFile[] = JSON.parse(
    readFileSync(join(htmlDirectory, manifestFileName), "utf8"),
  );

  for (const [folderName, width, height] of widths) {
    await captureAtWidth(browser, previews, folderName, width, height);
  }
});

function renderEmailPreviews(): void {
  const result = spawnSync(
    "dotnet",
    ["run", "--project", emailPreviewsProject, "--", htmlDirectory],
    {
      stdio: "inherit",
    },
  );
  if (result.status !== 0) {
    throw new Error(`The email previews tool failed with exit code ${result.status}.`);
  }
}

async function captureAtWidth(
  browser: Browser,
  previews: EmailPreviewFile[],
  folderName: string,
  width: number,
  height: number,
): Promise<void> {
  const context = await browser.newContext({ viewport: { width, height }, locale: "es-AR" });
  await context.route(webFontRequests, (route) =>
    isWebFontBlocked
      ? route.abort()
      : route.fulfill({
          path: join(webAppFontsDirectory, basename(new URL(route.request().url()).pathname)),
          headers: { "Access-Control-Allow-Origin": "*" },
        }),
  );
  const page = await context.newPage();
  const screenshotsDirectory = join(emailsDirectory, folderName);
  mkdirSync(screenshotsDirectory, { recursive: true });

  for (const preview of previews) {
    await page.goto(pathToFileURL(join(htmlDirectory, preview.file)).href);
    await page.evaluate(() => document.fonts.ready);
    await page.screenshot({
      path: join(screenshotsDirectory, preview.file.replace(/\.html$/, ".png")),
      fullPage: true,
    });
  }
  await context.close();
}
