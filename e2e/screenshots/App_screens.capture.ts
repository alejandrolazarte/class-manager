import { expect, Page, test } from "@playwright/test";
import { mkdirSync } from "node:fs";
import { join } from "node:path";
import { e2eEnvironment } from "../support/environment";
import { SmtpSink } from "../support/smtpSink";
import { demoPassword, demoTimeZoneId, seedDemoBusiness } from "./demoBusiness";
import { waitForBrandWelcomeToClose } from "./brandWelcome";

const outputDirectory = process.env.SCREENSHOTS_DIR ?? "screenshots-output";
const settleMilliseconds = 800;
const darkColorScheme = "dark";
const colorScheme = process.env.SCREENSHOTS_COLOR_SCHEME === darkColorScheme ? "dark" : "light";
const wrongPassword = "not-the-right-password";

test.use({ locale: "es-AR", timezoneId: demoTimeZoneId, colorScheme });

const smtpSink = new SmtpSink();

test.beforeAll(async () => {
  await smtpSink.start(e2eEnvironment.smtpPort);
});

test.afterAll(async () => {
  await smtpSink.stop();
});

test("App screens", async ({ page, request }) => {
  const demo = await seedDemoBusiness(request);
  mkdirSync(outputDirectory, { recursive: true });

  await page.goto("/welcome");
  await page.getByText("Registrá tu negocio").filter({ visible: true }).first().waitFor();
  await capture(page, "00a-bienvenida");

  await page.goto("/sign-in");
  await page.getByText("Ingresá a tu negocio").filter({ visible: true }).first().waitFor();
  await capture(page, "00b-ingresar");

  await page.getByLabel("Email").fill(demo.email);
  await page.getByLabel("Contraseña", { exact: true }).fill(wrongPassword);
  await page.getByRole("button", { name: "Ingresar" }).click();
  await page
    .getByText("Email o contraseña incorrectos")
    .filter({ visible: true })
    .first()
    .waitFor();
  await capture(page, "00c-ingresar-error");

  await page.getByText("¿Olvidaste tu contraseña?").click();
  await page.getByText("Recuperá tu contraseña").filter({ visible: true }).first().waitFor();
  await capture(page, "00d-olvide-mi-contrasena");

  await page.getByRole("button", { name: "Enviar link" }).click();
  await page.getByText("Volver a ingresar").filter({ visible: true }).first().waitFor();
  await capture(page, "00e-olvide-mi-contrasena-enviado");

  await page.goto("/sign-up");
  await page.getByLabel("Nombre y apellido").fill("Valeria Sosa");
  await page.getByLabel("Fecha de nacimiento").fill("20061985");
  await page.getByLabel("Email").fill("valeria.sosa@example.com");
  await page.getByLabel("Contraseña", { exact: true }).fill(demoPassword);
  await capture(page, "00f-registro-tu-cuenta");

  await page.getByRole("button", { name: "Continuar" }).click();
  await page.getByLabel("Nombre del negocio").fill("Estudio Valeria");
  await capture(page, "00g-registro-tu-negocio");

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
    ["06-inscribir", `/classes/${demo.beginnersClassGroupId}/enroll`, "Ya está en la clase"],
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
    ["14-alumno-por-clases", `/fees/clients/${demo.classPackClientId}`, "Quedan"],
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

  await page.goto("/today");
  await page.getByText("Próxima clase").filter({ visible: true }).first().waitFor();
  await page.getByRole("button", { name: "Mes", exact: true }).click();
  await page.getByText("Elegí un día").filter({ visible: true }).first().waitFor();
  await capture(page, "01b-inicio-mes");

  await page.goto(`/today/private/${demo.privateLessonId}`);
  await page.getByText("Valentina Fernández").filter({ visible: true }).first().waitFor();
  await capture(page, "01d-clase-particular");

  await page.goto(`/classes/${demo.beginnersClassGroupId}`);
  await page.getByRole("button", { name: "Dar de baja a Tomás Pérez" }).click();
  await page.getByText("¿Dar de baja a Tomás Pérez?").filter({ visible: true }).first().waitFor();
  await capture(page, "04b-confirmar-dar-de-baja");
  await page.getByRole("alert").getByRole("button", { name: "Volver" }).click();

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
    .getByLabel("Detalle")
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
  await page.getByText("Automático").filter({ visible: true }).first().waitFor();
  await page.waitForTimeout(settleMilliseconds);
  await waitForBrandWelcomeToClose(page);
  await page.screenshot({ path: join(outputDirectory, "18-apariencia.png") });

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
    await chooseBusinessTheme(page, themeName);
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

async function capture(page: Page, fileName: string): Promise<void> {
  await page.waitForTimeout(settleMilliseconds);
  await waitForBrandWelcomeToClose(page);
  await page.screenshot({ path: join(outputDirectory, `${fileName}.png`) });
}

async function chooseBusinessTheme(page: Page, themeName: string): Promise<void> {
  await page.goto("/settings/brand");
  await waitForBrandWelcomeToClose(page);
  await page.getByRole("button", { name: themeName, exact: true }).click();
  await page.getByRole("button", { name: "Guardar marca" }).click();
  await page.getByText("Marca guardada", { exact: false }).waitFor();
}

async function openAppearance(page: Page): Promise<void> {
  await page.goto("/settings");
  await page.getByRole("button", { name: "Apariencia" }).click();
}
