import { fireEvent, screen } from "@testing-library/react-native";
import { BackofficeRequestError } from "@/backoffice/api/backofficeHttpClient";
import { GiveAddOnScreen } from "@/backoffice/brands/GiveAddOnScreen";
import { getBrand } from "@/backoffice/brands/brandsApi";
import { getSubscriptionCatalog, giveBrandAddOn } from "@/backoffice/brands/brandSubscriptionApi";
import { translate } from "@/i18n/translate";
import { aBrandDetail, aSubscriptionCatalog } from "@/testing/backofficeBrands";
import { renderBackofficeScreen } from "@/testing/renderBackofficeScreen";

jest.mock("@/backoffice/brands/brandsApi");
jest.mock("@/backoffice/brands/brandSubscriptionApi");

describe("When the server refuses the price", () => {
  beforeEach(() => {
    jest.mocked(getBrand).mockResolvedValue(aBrandDetail());
    jest.mocked(getSubscriptionCatalog).mockResolvedValue(aSubscriptionCatalog());
    jest.mocked(giveBrandAddOn).mockRejectedValue(
      new BackofficeRequestError(400, {
        status: 400,
        code: "backoffice.negative_price",
        errors: { price: ["A price can't be negative."] },
      }),
    );
  });

  it("Then the price field shows why", async () => {
    await renderBackofficeScreen(<GiveAddOnScreen brandId="brand-1" />);

    await fireEvent.press(await screen.findByText(translate("subscriptions.feature.shop")));
    await fireEvent.press(
      screen.getByRole("button", { name: translate("backoffice.giveAddOn.submit") }),
    );

    expect(
      await screen.findByText(translate("apiErrors.backoffice.negative_price")),
    ).toBeOnTheScreen();
  });
});
