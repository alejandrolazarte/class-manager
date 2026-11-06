import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { getBrand } from "@/backoffice/brands/brandsApi";
import { BrandSalesFeeScreen } from "@/backoffice/salesFees/BrandSalesFeeScreen";
import { getBrandSalesFee, giveBrandSalesFee } from "@/backoffice/salesFees/salesFeesApi";
import { translate } from "@/i18n/translate";
import { aBrandDetail } from "@/testing/backofficeBrands";
import { renderBackofficeScreen } from "@/testing/renderBackofficeScreen";

jest.mock("@/backoffice/brands/brandsApi");
jest.mock("@/backoffice/salesFees/salesFeesApi");

describe("When the operator gives a brand its own sales fee", () => {
  beforeEach(() => {
    jest.mocked(getBrand).mockResolvedValue(aBrandDetail());
    jest
      .mocked(getBrandSalesFee)
      .mockResolvedValue({ percent: 0, isBrandFee: false, planPercent: 0, note: null });
    jest
      .mocked(giveBrandSalesFee)
      .mockResolvedValue({ percent: 5, isBrandFee: true, planPercent: 0, note: null });
  });

  it("Then nothing changes until they confirm", async () => {
    await renderBackofficeScreen(<BrandSalesFeeScreen brandId="brand-1" />);

    await fireEvent.changeText(
      await screen.findByLabelText(translate("backoffice.salesFee.percent"), { exact: false }),
      "5",
    );
    await fireEvent.press(
      screen.getByRole("button", { name: translate("backoffice.salesFee.submit") }),
    );
    await screen.findByText(translate("backoffice.salesFee.confirmMessage"));
    expect(giveBrandSalesFee).not.toHaveBeenCalled();

    await fireEvent.press(
      screen.getByRole("button", { name: translate("backoffice.salesFee.confirm") }),
    );

    await waitFor(() =>
      expect(giveBrandSalesFee).toHaveBeenCalledWith("brand-1", { percent: 5, note: null }),
    );
  });
});
