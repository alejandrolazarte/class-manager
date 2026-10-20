import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { BrandScreen } from "@/backoffice/brands/BrandScreen";
import { getBrand } from "@/backoffice/brands/brandsApi";
import { removeBrandAddOn, restoreBrandAddOn } from "@/backoffice/brands/brandSubscriptionApi";
import { translate } from "@/i18n/translate";
import { aBrandDetail } from "@/testing/backofficeBrands";
import { signInToBackofficeAs } from "@/testing/backofficeSession";
import { renderBackofficeScreen } from "@/testing/renderBackofficeScreen";

jest.mock("@/backoffice/brands/brandsApi");
jest.mock("@/backoffice/brands/brandSubscriptionApi");
jest.mock("@/backoffice/session/backofficeSessionApi");

const addOn = {
  id: "add-on-1",
  featureCode: "branches",
  price: 10,
  currency: "EUR",
  limit: 4,
  createdOn: "2026-10-10T10:00:00Z",
  expiredOn: null,
};

describe("When an operator removes an add-on", () => {
  beforeEach(() => {
    signInToBackofficeAs("PlatformOperator");
    jest.mocked(getBrand).mockResolvedValue(aBrandDetail({ addOns: [addOn] }));
    jest.mocked(removeBrandAddOn).mockResolvedValue(addOn);
    jest.mocked(restoreBrandAddOn).mockResolvedValue(addOn);
  });

  it("Then undo restores it", async () => {
    await renderBackofficeScreen(<BrandScreen brandId="brand-1" />);

    await fireEvent.press(
      await screen.findByRole("button", {
        name: translate("backoffice.brand.removeAddOn", {
          feature: translate("subscriptions.feature.branches"),
        }),
      }),
    );
    await fireEvent.press(await screen.findByText(translate("common.undo")));

    expect(removeBrandAddOn).toHaveBeenCalledWith("brand-1", "add-on-1");
    await waitFor(() => expect(restoreBrandAddOn).toHaveBeenCalledWith("brand-1", "add-on-1"));
  });
});
