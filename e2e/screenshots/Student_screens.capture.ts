import { expect, Page, test } from "@playwright/test";
import { mkdirSync } from "node:fs";
import { join } from "node:path";
import { e2eEnvironment } from "../support/environment";
import { SmtpSink } from "../support/smtpSink";
import { demoPassword, demoTimeZoneId, seedDemoBusiness } from "./demoBusiness";
import {
  inviteOwnerAsStudent,
  demoStudentAppPassword,
  seedDemoStudentApp,
  weekEndOf,
} from "./demoStudentApp";
import { waitForBrandWelcomeToClose } from "./brandWelcome";

const outputDirectory = process.env.SCREENSHOTS_DIR ?? "screenshots-output";
const settleMilliseconds = 800;
const scrollDistancePixels = 2000;
const darkColorScheme = "dark";
const colorScheme = process.env.SCREENSHOTS_COLOR_SCHEME === darkColorScheme ? "dark" : "light";

test.use({
  locale: "es-AR",
  timezoneId: demoTimeZoneId,
  colorScheme,
  permissions: ["notifications"],
});

const smtpSink = new SmtpSink();

test.beforeAll(async () => {
  await smtpSink.start(e2eEnvironment.smtpPort);
});

test.afterAll(async () => {
  await smtpSink.stop();
});

const shortWeekdays = ["Dom", "Lun", "Mar", "Mié", "Jue", "Vie", "Sáb"];

async function openClassesDay(
  page: Page,
  isoDate: string,
  today: string,
  studentName = "Tomás",
): Promise<void> {
  await page.goto("/student-app/classes");
  await page.getByText("Hoy,", { exact: false }).filter({ visible: true }).first().waitFor();
  if (isoDate > weekEndOf(today)) {
    await page.getByRole("button", { name: "Semana siguiente" }).click();
  }
  await page.getByRole("button", { name: studentName }).click();
  const day = new Date(`${isoDate}T12:00:00Z`);
  await page
    .getByRole("button", {
      name: `${shortWeekdays[day.getUTCDay()]} ${day.getUTCDate()}`,
      exact: true,
    })
    .click();
}

async function capture(page: Page, fileName: string): Promise<void> {
  await page.waitForTimeout(settleMilliseconds);
  await waitForBrandWelcomeToClose(page);
  await page.screenshot({ path: join(outputDirectory, `${fileName}.png`) });
}

async function scrollToBottom(page: Page): Promise<void> {
  const viewport = page.viewportSize();
  await page.mouse.move((viewport?.width ?? 0) / 2, (viewport?.height ?? 0) / 2);
  await page.mouse.wheel(0, scrollDistancePixels);
}

async function askForNotificationsLikeAFreshBrowser(page: Page): Promise<void> {
  await page.addInitScript(() => {
    Object.defineProperty(Notification, "permission", { get: () => "default" });
  });
}

test("Student screens", async ({ page, request }) => {
  await askForNotificationsLikeAFreshBrowser(page);
  const demo = await seedDemoBusiness(request);
  const student = await seedDemoStudentApp(request, demo, smtpSink);
  mkdirSync(outputDirectory, { recursive: true });

  await page.goto("/sign-in");
  await page.getByLabel("Email").fill(student.email);
  await page.getByLabel("Contraseña", { exact: true }).fill(demoStudentAppPassword);
  await page.getByRole("button", { name: "Ingresar" }).click();
  await expect(page).toHaveURL(/\/student-app/);

  await page.getByText("Próxima clase").filter({ visible: true }).first().waitFor();
  await capture(page, "20-alumno-inicio");

  await page.getByRole("button", { name: "Tomás" }).click();
  await capture(page, "21-alumno-inicio-otro-alumno");

  await page.getByRole("button", { name: /^Novedades/ }).click();
  await page.getByText("Lunes 12 de octubre cerrado").filter({ visible: true }).first().waitFor();
  await capture(page, "21b-alumno-novedades");

  await page.goto("/student-app/classes");
  await page.getByText("Hoy,", { exact: false }).filter({ visible: true }).first().waitFor();
  await capture(page, "22-alumno-clases");

  await openClassesDay(page, student.absenceDate, demo.today);
  await page.getByText("No va", { exact: true }).filter({ visible: true }).first().waitFor();
  await capture(page, "22b-alumno-clases-no-voy");

  await openClassesDay(page, student.makeupDate, demo.today);
  await page.getByText("Recuperación", { exact: true }).filter({ visible: true }).first().waitFor();
  await capture(page, "22c-alumno-clase-de-recuperacion");
  await scrollToBottom(page);
  await page.getByText("Recuperar clases").filter({ visible: true }).first().waitFor();
  await capture(page, "22d-alumno-recuperar-clases");

  await openClassesDay(page, student.packClassDate, demo.today, "Lucía");
  await page.getByText("Reservada", { exact: true }).filter({ visible: true }).first().waitFor();
  await capture(page, "22e-alumno-clase-del-pack");
  await page.getByText("Reservar clases del pack").filter({ visible: true }).first().waitFor();

  await page.goto("/student-app/shop");
  await page.getByText("Productos").filter({ visible: true }).first().waitFor();
  await capture(page, "23-alumno-tienda");
  await scrollToBottom(page);
  await capture(page, "23b-alumno-tienda-productos");

  await page.getByRole("button", { name: "Remera del club" }).click();
  await page.getByText("Talle").filter({ visible: true }).first().waitFor();
  await capture(page, "24-alumno-producto");

  await page.getByRole("button", { name: "M", exact: true }).click();
  await page.getByRole("button", { name: /^Agregar ·/ }).click();
  await page.getByRole("button", { name: /Agregar 8 clases/ }).click();
  await capture(page, "25-alumno-tienda-con-carrito");

  await page.getByRole("button", { name: /Ver carrito/ }).click();
  await page.getByText("Tu carrito").waitFor();
  await capture(page, "26-alumno-carrito");

  await page.goto("/student-app/orders");
  await page.getByText("Listo para entregar").filter({ visible: true }).first().waitFor();
  await capture(page, "27-alumno-pedidos");

  await page.getByRole("button", { name: /Anteriores/ }).click();
  await page.getByText("Cancelado").filter({ visible: true }).first().waitFor();
  await capture(page, "28-alumno-pedidos-anteriores");

  await page.goto("/student-app/progress");
  await page.getByText("Camino de niveles").filter({ visible: true }).first().waitFor();
  await capture(page, "28b-alumno-logros");
  await scrollToBottom(page);
  await capture(page, "28c-alumno-logros-medallas");

  await page.goto("/student-app/settings");
  await page.getByRole("button", { name: "Notificaciones" }).waitFor();
  await capture(page, "29-alumno-ajustes");

  await page.getByRole("button", { name: "Notificaciones" }).click();
  await page.getByRole("switch", { name: "Avisarme de novedades" }).waitFor();
  await capture(page, "29b-alumno-ajustes-notificaciones");
  await page.getByRole("button", { name: "Listo" }).click();

  await page.getByRole("button", { name: "Apariencia" }).click();
  await page.getByText("Colores").filter({ visible: true }).first().waitFor();
  await capture(page, "29c-alumno-ajustes-apariencia");
});

test("Account chooser", async ({ page, request }) => {
  const demo = await seedDemoBusiness(request);
  await inviteOwnerAsStudent(request, demo, smtpSink);
  mkdirSync(outputDirectory, { recursive: true });

  await page.goto("/sign-in");
  await page.getByLabel("Email").fill(demo.email);
  await page.getByLabel("Contraseña", { exact: true }).fill(demoPassword);
  await page.getByRole("button", { name: "Ingresar" }).click();
  await expect(page).toHaveURL(/\/choose-account/);
  await page.getByText("¿Cómo querés entrar?").filter({ visible: true }).first().waitFor();
  await capture(page, "30-elegir-cuenta");

  await page.getByRole("button", { name: /^Alumno en/ }).click();
  await expect(page).toHaveURL(/\/student-app/);
  await page.getByText("Próxima clase").filter({ visible: true }).first().waitFor();
});
