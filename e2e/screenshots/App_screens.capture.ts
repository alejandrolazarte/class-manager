import { expect, Page, test } from "@playwright/test";
import { mkdirSync } from "node:fs";
import { join } from "node:path";
import { demoPassword, demoTimeZoneId, seedDemoBusiness } from "./demoBusiness";
import { waitForBrandWelcomeToClose } from "./brandWelcome";

const outputDirectory = process.env.SCREENSHOTS_DIR ?? "screenshots-output";
const settleMilliseconds = 800;
const darkColorScheme = "dark";
const colorScheme = process.env.SCREENSHOTS_COLOR_SCHEME === darkColorScheme ? "dark" : "light";

test.use({ locale: "es-AR", timezoneId: demoTimeZoneId, colorScheme });

test("App screens", async ({ page, request }) => {
  const demo = await seedDemoBusiness(request);
  mkdirSync(outputDirectory, { recursive: true });

  await page.goto("/sign-in");
  await page.getByLabel("Email").fill(demo.email);
  await page.getByLabel("Contraseña", { exact: true }).fill(demoPassword);
  await page.getByRole("button", { name: "Ingresar" }).click();
  await expect(page).toHaveURL(/\/today/);

  const screens: [fileName: string, path: string, visibleText: string][] = [
    ["01-inicio", "/today", "Próxima clase"],
    ["02-asistencia", `/today/${demo.beginnersClassGroupId}/${demo.today}`, "Vino"],
    ["03-clases-semana", "/classes", "lugares"],
    ["04-clase-alumnos", `/classes/${demo.beginnersClassGroupId}`, "Inscribir alumno"],
    ["05-clase-editar", `/classes/${demo.beginnersClassGroupId}/edit`, "Nombre de la clase"],
    ["06-inscribir", `/classes/${demo.beginnersClassGroupId}/enroll`, "Ya inscripto"],
    ["07-alumnos", "/students", "Tomás Pérez"],
    ["08-alumno", `/students/clients/${demo.studentAppClientId}`, "Natación inicial"],
    ["09-nuevo-alumno", "/students/new", "¿Quién viene a clase?"],
    ["10-cuotas", `/fees?month=${demo.month}`, "Cobrado"],
    [
      "11-registrar-pago",
      `/fees/clients/${demo.debtorClientId}/pay?month=${demo.month}`,
      "Forma de pago",
    ],
    ["12-ajustes", "/settings", "Cuota mensual"],
    ["13-profes", "/settings/instructors", "Martín Díaz"],
    ["14-alumno-por-clases", `/students/clients/${demo.classPackClientId}`, "Quedan"],
    ["15-vender-pack", `/students/clients/${demo.classPackClientId}/sell-pack`, "Forma de pago"],
    ["16-packs", "/settings/class-packs", "8 clases"],
    ["16b-pack-nuevo", "/settings/class-packs/new", "Clases donde se puede usar"],
    ["17-cuota-mensual", "/settings/monthly-fee", "Cambios de cuota"],
  ];

  for (const [fileName, path, visibleText] of screens) {
    await page.goto(path);
    await page
      .getByText(visibleText, { exact: false })
      .filter({ visible: true })
      .filter({ visible: true })
      .first()
      .waitFor();
    await page.waitForTimeout(settleMilliseconds);
    await waitForBrandWelcomeToClose(page);
    await page.screenshot({ path: join(outputDirectory, `${fileName}.png`) });
  }

  await page.goto(`/today/${demo.beginnersClassGroupId}/${demo.today}`);
  await page.getByRole("button", { name: "Comentario para Tomás Pérez" }).click();
  await page
    .getByLabel("Qué tal le fue en la clase")
    .fill("Muy buen ritmo en la serie larga. La semana que viene probamos la salida.");
  await page.waitForTimeout(settleMilliseconds);
  await waitForBrandWelcomeToClose(page);
  await page.screenshot({ path: join(outputDirectory, "02b-comentario-del-profe.png") });

  await page.goto("/settings/announcements");
  await page.getByLabel("Título").fill("Lunes 12 de octubre cerrado");
  await page
    .getByLabel("Detalle (opcional)")
    .fill("Feriado. Las clases de ese día se pueden recuperar durante la semana.");
  await page.waitForTimeout(settleMilliseconds);
  await waitForBrandWelcomeToClose(page);
  await page.screenshot({ path: join(outputDirectory, "12b-novedades.png") });

  await page.goto("/settings/business");
  await page.getByText("Moneda").filter({ visible: true }).first().waitFor();
  await page.waitForTimeout(settleMilliseconds);
  await waitForBrandWelcomeToClose(page);
  await page.screenshot({ path: join(outputDirectory, "12c-ajustes-negocio.png") });

  await page.goto("/settings/achievements");
  await page.getByRole("switch", { name: "Faltar con aviso no corta la racha" }).waitFor();
  await page.waitForTimeout(settleMilliseconds);
  await waitForBrandWelcomeToClose(page);
  await page.screenshot({ path: join(outputDirectory, "12d-logros-y-asistencia.png") });

  await page.goto(`/fees?month=${demo.month}`);
  await page.getByRole("button", { name: "Todos" }).click();
  await page.waitForTimeout(settleMilliseconds);
  await waitForBrandWelcomeToClose(page);
  await page.screenshot({ path: join(outputDirectory, "10b-cuotas-todos.png") });

  await page.goto("/fees?view=orders");
  await page
    .getByText(/para entregar$/)
    .filter({ visible: true })
    .first()
    .waitFor();
  await page.waitForTimeout(settleMilliseconds);
  await waitForBrandWelcomeToClose(page);
  await page.screenshot({ path: join(outputDirectory, "10c-cobros-pedidos.png") });

  await openAppearance(page);
  await page.getByRole("button", { name: "Grafito", exact: true }).scrollIntoViewIfNeeded();
  await page.waitForTimeout(settleMilliseconds);
  await waitForBrandWelcomeToClose(page);
  await page.screenshot({ path: join(outputDirectory, "18-colores.png") });

  const themes: [fileName: string, themeName: string][] = [
    ["19-hoy-tema-violeta", "Violeta"],
    ["19-hoy-tema-oceano", "Océano"],
    ["19-hoy-tema-rosa", "Rosa"],
    ["19-hoy-tema-ciruela", "Ciruela"],
    ["19-hoy-tema-indigo", "Índigo"],
    ["19-hoy-tema-grafito", "Grafito"],
    ["19-hoy-tema-agua", "Agua"],
  ];

  for (const [fileName, themeName] of themes) {
    await openAppearance(page);
    await page.getByRole("button", { name: themeName, exact: true }).click();
    await page.goto("/today");
    await page
      .getByText("Próxima clase")
      .filter({ visible: true })
      .filter({ visible: true })
      .first()
      .waitFor();
    await page.waitForTimeout(settleMilliseconds);
    await waitForBrandWelcomeToClose(page);
    await page.screenshot({ path: join(outputDirectory, `${fileName}.png`) });
  }
});

async function openAppearance(page: Page): Promise<void> {
  await page.goto("/settings");
  await page.getByRole("button", { name: "Apariencia" }).click();
}
