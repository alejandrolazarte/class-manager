import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { ChangePlanScreen } from "@/backoffice/brands/ChangePlanScreen";
import { getBrand } from "@/backoffice/brands/brandsApi";
import { changeBrandPlan, getSubscriptionCatalog } from "@/backoffice/brands/brandSubscriptionApi";
import { translate } from "@/i18n/translate";
import { aBrandDetail, aSubscriptionCatalog } from "@/testing/backofficeBrands";
import { renderBackofficeScreen } from "@/testing/renderBackofficeScreen";

jest.mock("@/backoffice/brands/brandsApi");
jest.mock("@/backoffice/brands/brandSubscriptionApi");

describe("When the operator changes the plan", () => {
  beforeEach(() => {
    jest.mocked(getBrand).mockResolvedValue(aBrandDetail());
    jest.mocked(getSubscriptionCatalog).mockResolvedValue(aSubscriptionCatalog());
    jest.mocked(changeBrandPlan).mockResolvedValue({ subscriptionId: "subscription-2" });
  });

  it("Then nothing changes until they confirm", async () => {
    await renderBackofficeScreen(<ChangePlanScreen brandId="brand-1" />);

    await fireEvent.press(await screen.findByText(translate("subscriptions.plan.enterprise")));
    await fireEvent.changeText(
      screen.getByLabelText(translate("backoffice.subscriptionTerms.price"), { exact: false }),
      "99,50",
    );
    await fireEvent.press(
      screen.getByRole("button", { name: translate("backoffice.changePlan.submit") }),
    );
    await screen.findByText(translate("backoffice.changePlan.confirmMessage"));
    expect(changeBrandPlan).not.toHaveBeenCalled();

    await fireEvent.press(
      screen.getByRole("button", { name: translate("backoffice.changePlan.confirm") }),
    );

    await waitFor(() =>
      expect(changeBrandPlan).toHaveBeenCalledWith("brand-1", {
        planCode: "enterprise",
        price: 99.5,
        currency: "EUR",
        note: null,
        expiredOn: null,
      }),
    );
  });
});
