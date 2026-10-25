import { CDPSession, Page, test } from "@playwright/test";
import { mkdirSync } from "node:fs";
import { join } from "node:path";
import { waitForBrandWelcomeToClose } from "./brandWelcome";
import { demoPassword, demoTimeZoneId, seedDemoBusiness } from "./demoBusiness";

const outputDirectory = process.env.SCREENSHOTS_DIR ?? "screenshots-output";
const settleMilliseconds = 800;
const darkColorScheme = "dark";
const colorScheme = process.env.SCREENSHOTS_COLOR_SCHEME === darkColorScheme ? "dark" : "light";
const passkeyName = "iPhone de Laura";

test.use({ locale: "es-AR", timezoneId: demoTimeZoneId, colorScheme });

test("Passkey screens", async ({ page, request }) => {
  const demo = await seedDemoBusiness(request);
  mkdirSync(outputDirectory, { recursive: true });
  await addVirtualAuthenticator(await page.context().newCDPSession(page));

  await page.goto("/sign-in");
  await page.getByText("Ingresar con passkey").filter({ visible: true }).first().waitFor();
  await capture(page, "30-ingresar-con-passkey");

  await page.getByLabel("Email").fill(demo.email);
  await page.getByLabel("Contraseña", { exact: true }).fill(demoPassword);
  await page.getByRole("button", { name: "Ingresar", exact: true }).click();
  await page.waitForURL(/\/today/);

  await page.goto("/settings/profile");
  await page.getByText("Passkeys").filter({ visible: true }).first().waitFor();
  await capture(page, "30b-mi-perfil-con-passkeys");

  await page.getByRole("button", { name: "Passkeys" }).click();
  await page
    .getByText("Todavía no agregaste ninguna passkey.")
    .filter({ visible: true })
    .first()
    .waitFor();
  await capture(page, "30c-passkeys-vacio");

  await page.getByRole("button", { name: "Agregar passkey" }).click();
  await page.getByLabel("Contraseña actual").fill(demoPassword);
  await page.getByLabel("Nombre").fill(passkeyName);
  await capture(page, "30d-agregar-passkey");

  await page.getByRole("button", { name: "Agregar passkey" }).click();
  await page.getByText(passkeyName).filter({ visible: true }).first().waitFor();
  await capture(page, "30e-passkeys");

  await page.getByRole("button", { name: `Quitar la passkey ${passkeyName}` }).click();
  await page.getByText("¿Quitar esta passkey?").filter({ visible: true }).first().waitFor();
  await capture(page, "30f-quitar-passkey");
});

async function addVirtualAuthenticator(devTools: CDPSession): Promise<void> {
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
}

async function capture(page: Page, fileName: string): Promise<void> {
  await page.waitForTimeout(settleMilliseconds);
  await waitForBrandWelcomeToClose(page);
  await page.screenshot({ path: join(outputDirectory, `${fileName}.png`) });
}
