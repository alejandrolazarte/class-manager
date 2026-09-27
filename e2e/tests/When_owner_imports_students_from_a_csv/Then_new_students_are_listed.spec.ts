import { expect, signIn, test } from "../../support/fixtures";
import { signUpBusiness } from "../../support/businessApi";

const studentsCsv = [
  "Alumno;Teléfono;Fecha de nacimiento;Responsable",
  "Lucas Gómez;11 7777-8888;07/03/2015;María Gómez",
  ";11 1111-2222;;",
].join("\r\n");

test("Then new students are listed", async ({ page, request }) => {
  await signIn(page, await signUpBusiness(request));
  await page.goto("/settings/import-export");

  const fileChooserPromise = page.waitForEvent("filechooser");
  await page.getByRole("button", { name: "Elegir archivo CSV" }).click();
  const fileChooser = await fileChooserPromise;
  await fileChooser.setFiles({
    name: "alumnos.csv",
    mimeType: "text/csv",
    buffer: Buffer.from(studentsCsv, "utf8"),
  });

  await expect(page.getByText("1 para importar")).toBeVisible();
  await expect(page.getByText("Falta Alumno.")).toBeVisible();
  await page.getByRole("button", { name: "Importar 1 fila" }).click();
  await expect(page.getByText("Se importó 1 fila")).toBeVisible();

  await page.goto("/students");
  await expect(page.getByText("Lucas Gómez")).toBeVisible();
});
