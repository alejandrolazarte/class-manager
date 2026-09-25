import { expect, test } from "../../support/fixtures";
import { ownerPassword, uniqueOwnerEmail } from "../../support/businessApi";

test("Then clients list is shown", async ({ page }) => {
  await page.goto("/sign-up");
  await page.getByLabel("Nombre y apellido").fill("Valeria Sosa");
  await page.getByLabel("Email").fill(uniqueOwnerEmail());
  await page.getByLabel("Contraseña", { exact: true }).fill(ownerPassword);
  await page.getByLabel("Nombre del negocio").fill("Panadería Valeria");
  await page.getByRole("button", { name: "Crear cuenta" }).click();

  await expect(page).toHaveURL(/\/clients/);
  await expect(page.getByText("Todavía no tenés clientes")).toBeVisible();
});
