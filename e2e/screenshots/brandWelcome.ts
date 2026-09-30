import { Page } from "@playwright/test";

const brandWelcomeTestId = "brand-welcome";

export async function waitForBrandWelcomeToClose(page: Page): Promise<void> {
  await page.getByTestId(brandWelcomeTestId).waitFor({ state: "hidden" });
}
