import { expect, signIn, test } from "../../support/fixtures";
import { signUpBusiness } from "../../support/businessApi";

const orderNotificationUrl = "/today/orders";

test("Then home tab goes back to home", async ({ page, request }) => {
  await signIn(page, await signUpBusiness(request));

  await page.goto(orderNotificationUrl);

  await expect(page).toHaveURL(/\/fees\?view=orders/);
  await expect(page.getByText("Tienda")).toBeVisible();

  await page.getByRole("tab", { name: "Inicio" }).click();

  await expect(page).toHaveURL(/\/today$/);
  await expect(page.getByText("Próxima clase")).toBeVisible();
});
