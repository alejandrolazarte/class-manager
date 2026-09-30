import { expect, Page, test as base } from "@playwright/test";
import { BusinessAccount, SeededBusiness, seedBusiness } from "./businessApi";

export async function signIn(page: Page, account: BusinessAccount): Promise<void> {
  await page.goto("/sign-in");
  await page.getByLabel("Email").fill(account.email);
  await page.getByLabel("Contraseña", { exact: true }).fill(account.password);
  await page.getByRole("button", { name: "Ingresar" }).click();
  await expect(page).toHaveURL(/\/today/);
}

export const test = base.extend<{ business: SeededBusiness }>({
  business: async ({ page, request }, use) => {
    const business = await seedBusiness(request);
    await signIn(page, business);
    await use(business);
  },
});

export { expect };
