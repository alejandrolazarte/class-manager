import { expect, test } from "@playwright/test";
import { mkdirSync } from "node:fs";
import { join } from "node:path";
import { demoPassword, demoTimeZoneId, seedDemoBusiness } from "./demoBusiness";

const outputDirectory = process.env.SCREENSHOTS_DIR ?? "screenshots-output";
const settleMilliseconds = 800;

test.use({ locale: "es-AR", timezoneId: demoTimeZoneId });

test("App screens", async ({ page, request }) => {
  const demo = await seedDemoBusiness(request);
  mkdirSync(outputDirectory, { recursive: true });

  await page.goto("/sign-in");
  await page.getByLabel("Email").fill(demo.email);
  await page.getByLabel("Contraseña", { exact: true }).fill(demoPassword);
  await page.getByRole("button", { name: "Ingresar" }).click();
  await expect(page).toHaveURL(/\/students/);

  const screens: [fileName: string, path: string, visibleText: string][] = [
    ["01-hoy", "/today", "presentes"],
    ["02-asistencia", `/today/${demo.beginnersClassGroupId}/${demo.today}`, "Vino"],
    ["03-clases-semana", "/classes", "lugares"],
    ["04-clase-alumnos", `/classes/${demo.beginnersClassGroupId}`, "Inscribir alumno"],
    ["05-clase-editar", `/classes/${demo.beginnersClassGroupId}/edit`, "Nombre de la clase"],
    ["06-inscribir", `/classes/${demo.beginnersClassGroupId}/enroll`, "Ya inscripto"],
    ["07-alumnos", "/students", "Tomás Pérez"],
    ["08-familia", `/students/clients/${demo.familyClientId}`, "Natación inicial"],
    ["09-nuevo-alumno", "/students/new", "¿Quién viene a clase?"],
    ["10-cuotas", `/fees?month=${demo.month}`, "Cobrado"],
    ["11-registrar-pago", `/fees/${demo.debtorClientId}/pay?month=${demo.month}`, "Forma de pago"],
    ["12-ajustes", "/settings", "Cuota mensual"],
    ["13-profes", "/settings/instructors", "Martín Díaz"],
  ];

  for (const [fileName, path, visibleText] of screens) {
    await page.goto(path);
    await page.getByText(visibleText, { exact: false }).first().waitFor();
    await page.waitForTimeout(settleMilliseconds);
    await page.screenshot({ path: join(outputDirectory, `${fileName}.png`) });
  }

  await page.goto(`/fees?month=${demo.month}`);
  await page.getByRole("button", { name: "Todos" }).click();
  await page.waitForTimeout(settleMilliseconds);
  await page.screenshot({ path: join(outputDirectory, "10b-cuotas-todos.png") });
});
