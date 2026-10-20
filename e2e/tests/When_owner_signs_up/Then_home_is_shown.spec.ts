import { expect, test } from "../../support/fixtures";
import { ownerPassword, uniqueOwnerEmail } from "../../support/businessApi";
import { linkTokenSentTo } from "../../support/mailbox";
import { personal } from "../../support/personalData";

const completeSignUpLinkPattern = /complete-sign-up\?token=([^\s]+)/;

test("Then home is shown", async ({ page }) => {
  const email = uniqueOwnerEmail();
  await page.goto("/sign-up");
  await page.getByLabel("Nombre y apellido").fill(personal("Valeria Sosa"));
  await page.getByLabel("Fecha de nacimiento").fill("20061985");
  await page.getByLabel("Email").fill(email);
  await page.getByRole("button", { name: "Continuar" }).click();
  await page.getByLabel("Nombre del negocio").fill("Panadería Valeria");
  await page
    .getByRole("checkbox", { name: "Acepto los términos de uso y la política de privacidad" })
    .click();
  await page.getByRole("button", { name: "Crear cuenta" }).click();
  await expect(page.getByText("Revisá tu email")).toBeVisible();

  const token = await linkTokenSentTo(email, completeSignUpLinkPattern);
  await page.goto(`/complete-sign-up?token=${encodeURIComponent(token)}`);
  await page.getByLabel("Contraseña", { exact: true }).fill(ownerPassword);
  await page.getByRole("button", { name: "Guardar y entrar" }).click();

  await expect(page).toHaveURL(/\/today/);
  await expect(page.getByText("Hola, Valeria")).toBeVisible();
});
