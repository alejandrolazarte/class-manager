import { expect, signIn, test } from "../../support/fixtures";
import { signUpBusiness } from "../../support/businessApi";

test("Then home is shown", async ({ page, request }) => {
  const account = await signUpBusiness(request);
  const devTools = await page.context().newCDPSession(page);
  await devTools.send("WebAuthn.enable");
  await devTools.send("WebAuthn.addVirtualAuthenticator", {
    options: {
      protocol: "ctap2",
      transport: "internal",
      hasResidentKey: true,
      hasUserVerification: true,
      isUserVerified: true,
      automaticPresenceSimulation: true,
    },
  });
  await signIn(page, account);
  await page.goto("/settings/profile");
  await page.getByRole("button", { name: "Passkeys" }).click();
  await page.getByRole("button", { name: "Agregar passkey" }).click();
  await page.getByLabel("Contraseña actual").fill(account.password);
  await page.getByLabel("Nombre").fill("Navegador de pruebas");
  await page.getByRole("button", { name: "Agregar passkey" }).click();
  await expect(page.getByText("Navegador de pruebas")).toBeVisible();
  await page.context().clearCookies();
  await page.evaluate(() => window.localStorage.clear());
  await page.goto("/sign-in");

  await page.getByRole("button", { name: "Ingresar con passkey" }).click();

  await expect(page.getByText("Próxima clase")).toBeVisible();
});
