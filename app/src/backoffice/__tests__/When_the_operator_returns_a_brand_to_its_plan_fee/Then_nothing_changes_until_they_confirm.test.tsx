import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { BrandScreen } from "@/backoffice/brands/BrandScreen";
import { getBrand } from "@/backoffice/brands/brandsApi";
import { getBrandSalesFee, removeBrandSalesFee } from "@/backoffice/salesFees/salesFeesApi";
import { translate } from "@/i18n/translate";
import { aBrandDetail } from "@/testing/backofficeBrands";
import { signInToBackofficeAs } from "@/testing/backofficeSession";
import { renderBackofficeScreen } from "@/testing/renderBackofficeScreen";

jest.mock("@/backoffice/brands/brandsApi");
jest.mock("@/backoffice/salesFees/salesFeesApi");
jest.mock("@/backoffice/session/backofficeSessionApi");

describe("When the operator returns a brand to its plan fee", () => {
  beforeEach(() => {
    signInToBackofficeAs("PlatformOperator");
    jest.mocked(getBrand).mockResolvedValue(aBrandDetail());
    jest.mocked(getBrandSalesFee).mockResolvedValue({
      percent: 5,
      isBrandFee: true,
      planPercent: 0,
      note: "Enterprise sin cargo",
    });
    jest
      .mocked(removeBrandSalesFee)
      .mockResolvedValue({ percent: 0, isBrandFee: false, planPercent: 0, note: null });
  });

  it("Then nothing changes until they confirm", async () => {
    await renderBackofficeScreen(<BrandScreen brandId="brand-1" />);

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("backoffice.salesFee.backToPlan") }),
    );
    await screen.findByText(translate("backoffice.salesFee.backToPlanQuestion"));
    expect(removeBrandSalesFee).not.toHaveBeenCalled();

    await fireEvent.press(
      screen
        .getAllByRole("button", { name: translate("backoffice.salesFee.backToPlanConfirm") })
        .at(-1)!,
    );

    await waitFor(() => expect(removeBrandSalesFee).toHaveBeenCalledWith("brand-1"));
  });
});
