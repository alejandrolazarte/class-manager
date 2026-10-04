import { Browser, expect, Page, test } from "@playwright/test";
import { mkdirSync } from "node:fs";
import { join } from "node:path";
import { e2eEnvironment } from "../support/environment";
import { SmtpSink } from "../support/smtpSink";
import { waitForBrandWelcomeToClose } from "./brandWelcome";
import { demoPassword, demoTimeZoneId, seedDemoBusiness } from "./demoBusiness";
import { demoStudentAppPassword, seedDemoStudentApp } from "./demoStudentApp";

const outputDirectory = process.env.SCREENSHOTS_DIR ?? "screenshots-output";
const settleMilliseconds = 800;
const logoSizePixels = 512;
const brandName = "Brazada";
const mainColor = "#5b3fa0";
const accentColor = "#efb062";
const darkColorScheme = "dark";
const colorScheme = process.env.SCREENSHOTS_COLOR_SCHEME === darkColorScheme ? "dark" : "light";

const logoMarkup = `
<body style="margin:0">
  <svg xmlns="http://www.w3.org/2000/svg" width="${logoSizePixels}" height="${logoSizePixels}" viewBox="0 0 100 100">
    <rect width="100" height="100" fill="${mainColor}"/>
    <path d="M10 62 Q 25 50 40 62 T 70 62 T 100 62 V 100 H 0 V 62 Z" fill="${accentColor}"/>
    <circle cx="66" cy="32" r="12" fill="${accentColor}"/>
    <text x="12" y="46" font-family="Arial Black, sans-serif" font-size="30" font-weight="900" fill="white">B</text>
  </svg>
</body>`;

test.use({ locale: "es-AR", timezoneId: demoTimeZoneId, colorScheme });

const smtpSink = new SmtpSink();

test.beforeAll(async () => {
  await smtpSink.start(e2eEnvironment.smtpPort);
});

test.afterAll(async () => {
  await smtpSink.stop();
});

async function capture(page: Page, fileName: string): Promise<void> {
  await page.waitForTimeout(settleMilliseconds);
  await page.screenshot({ path: join(outputDirectory, `${fileName}.png`) });
}

async function renderLogo(browser: Browser): Promise<Buffer> {
  const logoPage = await browser.newPage({
    viewport: { width: logoSizePixels, height: logoSizePixels },
  });
  await logoPage.setContent(logoMarkup);
  const logo = await logoPage.locator("svg").screenshot({ type: "png" });
  await logoPage.close();
  return logo;
}

async function signIn(page: Page, email: string, password: string): Promise<void> {
  await page.goto("/sign-in");
  await page.getByLabel("Email").fill(email);
  await page.getByLabel("Contraseña", { exact: true }).fill(password);
  await page.getByRole("button", { name: "Ingresar" }).click();
}

test("Brand screens", async ({ browser, page, request }) => {
  const demo = await seedDemoBusiness(request);
  const student = await seedDemoStudentApp(request, demo, smtpSink);
  const logo = await renderLogo(browser);
  mkdirSync(outputDirectory, { recursive: true });

  await signIn(page, demo.email, demoPassword);
  await expect(page).toHaveURL(/\/today/);
  await page.goto("/settings/brand");
  await page.getByText("¡Qué bueno verte!").waitFor();
  await waitForBrandWelcomeToClose(page);
  await page.getByText("Color principal").filter({ visible: true }).first().waitFor();
  await capture(page, "40-marca-vacia");

  const fileChooserPromise = page.waitForEvent("filechooser");
  await page.getByRole("button", { name: "Subir" }).click();
  await (
    await fileChooserPromise
  ).setFiles({
    name: "logo.png",
    mimeType: "image/png",
    buffer: logo,
  });
  await page.getByText("Logo actualizado").waitFor();
  await page.getByLabel("Nombre que ven los alumnos").fill(brandName);
  await page.getByRole("button", { name: `Color principal ${mainColor}` }).click();
  await page.getByRole("button", { name: `Acento (opcional) ${accentColor}` }).click();
  await capture(page, "41-marca-logo-y-colores");
  await page.getByRole("switch", { name: "Usar siempre mi marca" }).click();
  await page.getByRole("button", { name: "Guardar marca" }).click();
  await page.getByText("Marca guardada", { exact: false }).waitFor();
  await page.mouse.wheel(0, 2000);
  await capture(page, "42-marca-guardada");

  await page.getByRole("button", { name: "Ver pantalla de bienvenida" }).click();
  await page.getByText("¡Qué bueno verte!").waitFor();
  await capture(page, "43-marca-vista-previa-bienvenida");

  await page.goto("/today");
  await page.getByText("¡Qué bueno verte!").waitFor();
  await page.waitForTimeout(settleMilliseconds / 2);
  await page.screenshot({ path: join(outputDirectory, "44-equipo-bienvenida.png") });
  await waitForBrandWelcomeToClose(page);
  await capture(page, "45-equipo-hoy-con-marca");

  await page.goto("/students");
  await page.getByText("¡Qué bueno verte!").waitFor();
  await waitForBrandWelcomeToClose(page);
  await capture(page, "46-equipo-alumnos-con-marca");

  const studentAppContext = await browser.newContext({ colorScheme, locale: "es-AR" });
  const studentAppPage = await studentAppContext.newPage();
  await signIn(studentAppPage, student.email, demoStudentAppPassword);
  await expect(studentAppPage).toHaveURL(/\/student-app/);
  await studentAppPage.getByText("¡Qué bueno verte!").waitFor();
  await studentAppPage.waitForTimeout(settleMilliseconds / 2);
  await studentAppPage.screenshot({ path: join(outputDirectory, "47-alumno-bienvenida.png") });
  await waitForBrandWelcomeToClose(studentAppPage);
  await studentAppPage.getByText("Próxima clase").filter({ visible: true }).first().waitFor();
  await capture(studentAppPage, "48-alumno-inicio-con-marca");
  await studentAppContext.close();
});
