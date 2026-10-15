import { expect, signIn, test } from "../../support/fixtures";
import { signUpBusiness } from "../../support/businessApi";

const refreshTokenCookieName = "class-manager-refresh-token";

test("Then the session is kept", async ({ page, request }) => {
  await signIn(page, await signUpBusiness(request));

  await page.reload();

  await expect(page.getByText("Próxima clase")).toBeVisible();
  const cookies = await page.context().cookies();
  const refreshTokenCookie = cookies.find((cookie) => cookie.name === refreshTokenCookieName);
  expect(refreshTokenCookie?.httpOnly).toBe(true);
});
