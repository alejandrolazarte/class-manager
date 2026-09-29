import { expect, Page, test } from "@playwright/test";
import { mkdirSync } from "node:fs";
import { join } from "node:path";
import { e2eEnvironment } from "../support/environment";
import { SmtpSink } from "../support/smtpSink";
import { demoTimeZoneId, seedDemoBusiness } from "./demoBusiness";
import { demoFamilyPassword, seedDemoFamily } from "./demoFamily";

const outputDirectory = process.env.SCREENSHOTS_DIR ?? "screenshots-output";
const settleMilliseconds = 800;
const scrollDistancePixels = 2000;
const darkColorScheme = "dark";
const colorScheme = process.env.SCREENSHOTS_COLOR_SCHEME === darkColorScheme ? "dark" : "light";

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

async function scrollToBottom(page: Page): Promise<void> {
  const viewport = page.viewportSize();
  await page.mouse.move((viewport?.width ?? 0) / 2, (viewport?.height ?? 0) / 2);
  await page.mouse.wheel(0, scrollDistancePixels);
}

test("Family screens", async ({ page, request }) => {
  const demo = await seedDemoBusiness(request);
  const family = await seedDemoFamily(request, demo, smtpSink);
  mkdirSync(outputDirectory, { recursive: true });

  await page.goto("/sign-in");
  await page.getByLabel("Email").fill(family.email);
  await page.getByLabel("Contraseña", { exact: true }).fill(demoFamilyPassword);
  await page.getByRole("button", { name: "Ingresar" }).click();
  await expect(page).toHaveURL(/\/family/);

  await page.getByText("Próxima clase").first().waitFor();
  await capture(page, "20-familia-inicio");

  await page.getByRole("button", { name: "Tomás" }).click();
  await capture(page, "21-familia-inicio-otro-alumno");

  await page.goto("/family/classes");
  await page.getByText("Hoy,", { exact: false }).first().waitFor();
  await capture(page, "22-familia-clases");

  await page.goto("/family/shop");
  await page.getByText("Productos").first().waitFor();
  await capture(page, "23-familia-tienda");
  await scrollToBottom(page);
  await capture(page, "23b-familia-tienda-productos");

  await page.getByRole("button", { name: "Remera del club" }).click();
  await page.getByText("Talle").first().waitFor();
  await capture(page, "24-familia-producto");

  await page.getByRole("button", { name: "M", exact: true }).click();
  await page.getByRole("button", { name: /^Agregar ·/ }).click();
  await page.getByRole("button", { name: /Agregar 8 clases/ }).click();
  await capture(page, "25-familia-tienda-con-carrito");

  await page.getByRole("button", { name: /Ver carrito/ }).click();
  await page.getByText("Tu carrito").waitFor();
  await capture(page, "26-familia-carrito");

  await page.goto("/family/orders");
  await page.getByText("Listo para entregar").first().waitFor();
  await capture(page, "27-familia-pedidos");

  await page.getByRole("button", { name: /Anteriores/ }).click();
  await page.getByText("Cancelado").first().waitFor();
  await capture(page, "28-familia-pedidos-anteriores");
});
