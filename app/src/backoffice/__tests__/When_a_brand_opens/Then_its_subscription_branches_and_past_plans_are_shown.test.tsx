import { screen } from "@testing-library/react-native";
import { BrandScreen } from "@/backoffice/brands/BrandScreen";
import { getBrand } from "@/backoffice/brands/brandsApi";
import { translate } from "@/i18n/translate";
import { aBrandDetail } from "@/testing/backofficeBrands";
import { signInToBackofficeAs } from "@/testing/backofficeSession";
import { renderBackofficeScreen } from "@/testing/renderBackofficeScreen";

jest.mock("@/backoffice/brands/brandsApi");
jest.mock("@/backoffice/session/backofficeSessionApi");

describe("When a brand opens", () => {
  beforeEach(() => {
    signInToBackofficeAs("PlatformReader");
    jest.mocked(getBrand).mockResolvedValue(aBrandDetail());
  });

  it("Then its subscription, branches and past plans are shown", async () => {
    await renderBackofficeScreen(<BrandScreen brandId="brand-1" />);

    expect(await screen.findByText("Precio de lanzamiento")).toBeOnTheScreen();
    expect(
      screen.getByText(translate("backoffice.brand.usageOf", { used: 1, limit: 3 })),
    ).toBeOnTheScreen();
    expect(screen.getByText(translate("subscriptions.plan.free"))).toBeOnTheScreen();
    expect(getBrand).toHaveBeenCalledWith("brand-1");
  });
});
