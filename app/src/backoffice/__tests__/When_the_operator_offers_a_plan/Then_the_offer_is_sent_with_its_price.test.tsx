import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { OfferPlanScreen } from "@/backoffice/brands/OfferPlanScreen";
import { getBrand } from "@/backoffice/brands/brandsApi";
import { getSubscriptionCatalog, offerBrandPlan } from "@/backoffice/brands/brandSubscriptionApi";
import { translate } from "@/i18n/translate";
import { aBrandDetail, aPlanOffer, aSubscriptionCatalog } from "@/testing/backofficeBrands";
import { renderBackofficeScreen } from "@/testing/renderBackofficeScreen";

jest.mock("@/backoffice/brands/brandsApi");
jest.mock("@/backoffice/brands/brandSubscriptionApi");

describe("When the operator offers a plan", () => {
  beforeEach(() => {
    jest.mocked(getBrand).mockResolvedValue(aBrandDetail());
    jest.mocked(getSubscriptionCatalog).mockResolvedValue(aSubscriptionCatalog());
    jest.mocked(offerBrandPlan).mockResolvedValue(aPlanOffer());
  });

  it("Then the offer is sent with its price", async () => {
    await renderBackofficeScreen(<OfferPlanScreen brandId="brand-1" />);

    await fireEvent.press(await screen.findByText(translate("subscriptions.plan.lite")));
    expect(screen.queryByText(translate("subscriptions.plan.enterprise"))).not.toBeOnTheScreen();
    await fireEvent.changeText(
      screen.getByLabelText(translate("backoffice.offerPlan.price"), { exact: false }),
      "10",
    );
    await fireEvent.press(
      screen.getByRole("button", { name: translate("backoffice.offerPlan.submit") }),
    );

    await waitFor(() =>
      expect(offerBrandPlan).toHaveBeenCalledWith("brand-1", {
        planCode: "lite",
        price: 10,
        note: null,
        expiredOn: null,
      }),
    );
  });
});
