import { expect, test } from "../../support/fixtures";
import { signUpBusiness } from "../../support/businessApi";

test("Then error is shown", async ({ page, request }) => {
  const account = await signUpBusiness(request);
  await page.goto("/sign-in");
  await page.getByLabel("Email").fill(account.email);
  await page.getByLabel("Contraseña", { exact: true }).fill("not-the-right-password");

  await page.getByRole("button", { name: "Ingresar" }).click();

  await expect(page.getByText("Email o contraseña incorrectos")).toBeVisible();
  await expect(page).toHaveURL(/\/sign-in/);
});
