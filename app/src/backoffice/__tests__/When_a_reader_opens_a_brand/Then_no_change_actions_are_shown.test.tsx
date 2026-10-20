import { screen } from "@testing-library/react-native";
import { BrandScreen } from "@/backoffice/brands/BrandScreen";
import { getBrand } from "@/backoffice/brands/brandsApi";
import { translate } from "@/i18n/translate";
import { aBrandDetail } from "@/testing/backofficeBrands";
import { signInToBackofficeAs } from "@/testing/backofficeSession";
import { renderBackofficeScreen } from "@/testing/renderBackofficeScreen";

jest.mock("@/backoffice/brands/brandsApi");
jest.mock("@/backoffice/session/backofficeSessionApi");

describe("When a reader opens a brand", () => {
  beforeEach(() => {
    signInToBackofficeAs("PlatformReader");
    jest.mocked(getBrand).mockResolvedValue(aBrandDetail());
  });

  it("Then no change actions are shown", async () => {
    await renderBackofficeScreen(<BrandScreen brandId="brand-1" />);

    await screen.findByText("Sede Centro");
    expect(
      screen.queryByRole("button", { name: translate("backoffice.brand.changePlan") }),
    ).not.toBeOnTheScreen();
    expect(
      screen.queryByRole("button", { name: translate("backoffice.brand.giveAddOn") }),
    ).not.toBeOnTheScreen();
  });
});
