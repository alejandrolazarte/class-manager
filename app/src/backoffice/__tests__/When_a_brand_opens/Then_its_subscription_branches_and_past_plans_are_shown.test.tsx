import { screen } from "@testing-library/react-native";
import { BrandScreen } from "@/backoffice/brands/BrandScreen";
import { getBrand } from "@/backoffice/brands/brandsApi";
import { translate } from "@/i18n/translate";
import { aBrandDetail } from "@/testing/backofficeBrands";
import { renderBackofficeScreen } from "@/testing/renderBackofficeScreen";

jest.mock("@/backoffice/brands/brandsApi");

describe("When a brand opens", () => {
  beforeEach(() => {
    jest.mocked(getBrand).mockResolvedValue(aBrandDetail());
  });

  it("Then its subscription, branches and past plans are shown", async () => {
    await renderBackofficeScreen(<BrandScreen brandId="brand-1" />);

    expect(await screen.findByText("Precio de lanzamiento")).toBeOnTheScreen();
    expect(screen.getByText("Sede Centro")).toBeOnTheScreen();
    expect(
      screen.getByText(
        translate("subscriptions.featureUsed", {
          feature: translate("subscriptions.feature.branches"),
          used: 1,
          limit: 3,
        }),
      ),
    ).toBeOnTheScreen();
    expect(screen.getByText(translate("subscriptions.plan.free"))).toBeOnTheScreen();
    expect(getBrand).toHaveBeenCalledWith("brand-1");
  });
});
