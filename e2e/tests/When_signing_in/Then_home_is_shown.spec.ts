import { expect, signIn, test } from "../../support/fixtures";
import { signUpBusiness } from "../../support/businessApi";

test("Then home is shown", async ({ page, request }) => {
  const account = await signUpBusiness(request);

  await signIn(page, account);

  await expect(page.getByText("Próxima clase")).toBeVisible();
});
