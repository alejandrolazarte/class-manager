import { screen } from "@testing-library/react-native";
import { BrandsScreen } from "@/backoffice/brands/BrandsScreen";
import { getBrands } from "@/backoffice/brands/brandsApi";
import { translate, translateCount } from "@/i18n/translate";
import { aBrandSummary } from "@/testing/backofficeBrands";
import { renderBackofficeScreen } from "@/testing/renderBackofficeScreen";

jest.mock("@/backoffice/brands/brandsApi");

describe("When the brands open", () => {
  beforeEach(() => {
    jest.mocked(getBrands).mockResolvedValue([
      aBrandSummary(),
      aBrandSummary({
        id: "brand-2",
        name: "Instructora Sur",
        planCode: "free",
        status: "Trial",
        trialDaysLeft: 11,
      }),
    ]);
  });

  it("Then each brand shows its plan and status", async () => {
    await renderBackofficeScreen(<BrandsScreen />);

    expect(await screen.findByText("Estudio Centro")).toBeOnTheScreen();
    expect(
      screen.getByText(
        translate("backoffice.brands.usage", {
          plan: translate("subscriptions.plan.pro"),
          branches: 2,
          students: 40,
        }),
      ),
    ).toBeOnTheScreen();
    expect(
      screen.getByText(translateCount("backoffice.brands.trialDaysLeft", 11)),
    ).toBeOnTheScreen();
  });
});
