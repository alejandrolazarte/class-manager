import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { BrandScreen } from "@/backoffice/brands/BrandScreen";
import { getBrand } from "@/backoffice/brands/brandsApi";
import { offerBrandPlan, withdrawBrandPlanOffer } from "@/backoffice/brands/brandSubscriptionApi";
import { translate } from "@/i18n/translate";
import { aBrandDetail, aPlanOffer } from "@/testing/backofficeBrands";
import { signInToBackofficeAs } from "@/testing/backofficeSession";
import { renderBackofficeScreen } from "@/testing/renderBackofficeScreen";

jest.mock("@/backoffice/brands/brandsApi");
jest.mock("@/backoffice/brands/brandSubscriptionApi");
jest.mock("@/backoffice/session/backofficeSessionApi");

describe("When an operator withdraws the offer", () => {
  beforeEach(() => {
    signInToBackofficeAs("PlatformOperator");
    jest.mocked(getBrand).mockResolvedValue(aBrandDetail({ offer: aPlanOffer() }));
    jest.mocked(withdrawBrandPlanOffer).mockResolvedValue(aPlanOffer());
    jest.mocked(offerBrandPlan).mockResolvedValue(aPlanOffer());
  });

  it("Then undo offers it again", async () => {
    await renderBackofficeScreen(<BrandScreen brandId="brand-1" />);

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("backoffice.brand.withdrawOffer") }),
    );
    await fireEvent.press(await screen.findByText(translate("common.undo")));

    expect(withdrawBrandPlanOffer).toHaveBeenCalledWith("brand-1");
    await waitFor(() =>
      expect(offerBrandPlan).toHaveBeenCalledWith("brand-1", {
        planCode: "lite",
        price: 10,
        note: "Precio de lanzamiento",
        expiredOn: "2026-11-30",
      }),
    );
  });
});
