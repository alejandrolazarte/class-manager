import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { GiveAddOnScreen } from "@/backoffice/brands/GiveAddOnScreen";
import { getBrand } from "@/backoffice/brands/brandsApi";
import { getSubscriptionCatalog, giveBrandAddOn } from "@/backoffice/brands/brandSubscriptionApi";
import { translate } from "@/i18n/translate";
import { aBrandDetail, aSubscriptionCatalog } from "@/testing/backofficeBrands";
import { renderBackofficeScreen } from "@/testing/renderBackofficeScreen";

jest.mock("@/backoffice/brands/brandsApi");
jest.mock("@/backoffice/brands/brandSubscriptionApi");

describe("When the operator gives an add-on", () => {
  beforeEach(() => {
    jest.mocked(getBrand).mockResolvedValue(aBrandDetail());
    jest.mocked(getSubscriptionCatalog).mockResolvedValue(aSubscriptionCatalog());
    jest.mocked(giveBrandAddOn).mockResolvedValue({
      id: "add-on-1",
      featureCode: "branches",
      price: 10,
      currency: "EUR",
      limit: 5,
      createdOn: "2026-10-20T10:00:00Z",
      expiredOn: null,
    });
  });

  it("Then it is sent with its limit", async () => {
    await renderBackofficeScreen(<GiveAddOnScreen brandId="brand-1" />);

    await fireEvent.press(await screen.findByText(translate("subscriptions.feature.branches")));
    await fireEvent.changeText(
      screen.getByLabelText(translate("backoffice.giveAddOn.limit"), { exact: false }),
      "5",
    );
    await fireEvent.press(
      screen.getByRole("button", { name: translate("backoffice.giveAddOn.submit") }),
    );

    await waitFor(() =>
      expect(giveBrandAddOn).toHaveBeenCalledWith("brand-1", {
        featureCode: "branches",
        price: 10,
        currency: "EUR",
        limit: 5,
        expiredOn: null,
      }),
    );
  });
});
