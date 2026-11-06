import { fireEvent, screen } from "@testing-library/react-native";
import { getSubscriptionCatalog } from "@/backoffice/brands/brandSubscriptionApi";
import { PlanSalesFeeScreen } from "@/backoffice/salesFees/PlanSalesFeeScreen";
import { translate } from "@/i18n/translate";
import { aSubscriptionCatalog } from "@/testing/backofficeBrands";
import { renderBackofficeScreen } from "@/testing/renderBackofficeScreen";

jest.mock("@/backoffice/brands/brandSubscriptionApi");

describe("When a sales fee is over 100 percent", () => {
  beforeEach(() => {
    jest.mocked(getSubscriptionCatalog).mockResolvedValue(aSubscriptionCatalog());
  });

  it("Then the form explains the range", async () => {
    await renderBackofficeScreen(<PlanSalesFeeScreen planCode="lite" />);

    const percentField = await screen.findByLabelText(translate("backoffice.salesFee.percent"), {
      exact: false,
    });
    await fireEvent.changeText(percentField, "150");
    await fireEvent(percentField, "blur");

    expect(await screen.findByText(translate("backoffice.validation.salesFee"))).toBeTruthy();
  });
});
