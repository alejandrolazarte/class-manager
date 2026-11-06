import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { getSubscriptionCatalog } from "@/backoffice/brands/brandSubscriptionApi";
import { PlanSalesFeeScreen } from "@/backoffice/salesFees/PlanSalesFeeScreen";
import { changePlanSalesFee } from "@/backoffice/salesFees/salesFeesApi";
import { translate } from "@/i18n/translate";
import { aSubscriptionCatalog } from "@/testing/backofficeBrands";
import { renderBackofficeScreen } from "@/testing/renderBackofficeScreen";

jest.mock("@/backoffice/brands/brandSubscriptionApi");
jest.mock("@/backoffice/salesFees/salesFeesApi");

describe("When the operator changes a plan sales fee", () => {
  beforeEach(() => {
    jest.mocked(getSubscriptionCatalog).mockResolvedValue(aSubscriptionCatalog());
    jest.mocked(changePlanSalesFee).mockResolvedValue({
      code: "lite",
      listPrice: 19,
      currency: "EUR",
      durationInDays: null,
      salesFeePercent: 1.5,
    });
  });

  it("Then nothing changes until they confirm", async () => {
    await renderBackofficeScreen(<PlanSalesFeeScreen planCode="lite" />);

    await fireEvent.changeText(
      await screen.findByLabelText(translate("backoffice.salesFee.percent"), { exact: false }),
      "1,5",
    );
    await fireEvent.press(
      screen.getByRole("button", { name: translate("backoffice.plans.saveSalesFee") }),
    );
    await screen.findByText(translate("backoffice.plans.confirmMessage"));
    expect(changePlanSalesFee).not.toHaveBeenCalled();

    await fireEvent.press(
      screen.getByRole("button", { name: translate("backoffice.plans.confirm") }),
    );

    await waitFor(() => expect(changePlanSalesFee).toHaveBeenCalledWith("lite", 1.5));
  });
});
