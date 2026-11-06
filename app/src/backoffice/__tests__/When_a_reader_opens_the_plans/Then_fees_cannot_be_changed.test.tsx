import { screen } from "@testing-library/react-native";
import { getSubscriptionCatalog } from "@/backoffice/brands/brandSubscriptionApi";
import { PlansScreen } from "@/backoffice/salesFees/PlansScreen";
import { translate } from "@/i18n/translate";
import { aSubscriptionCatalog } from "@/testing/backofficeBrands";
import { signInToBackofficeAs } from "@/testing/backofficeSession";
import { renderBackofficeScreen } from "@/testing/renderBackofficeScreen";

jest.mock("@/backoffice/brands/brandSubscriptionApi");
jest.mock("@/backoffice/session/backofficeSessionApi");

describe("When a reader opens the plans", () => {
  beforeEach(() => {
    signInToBackofficeAs("PlatformReader");
    jest.mocked(getSubscriptionCatalog).mockResolvedValue(aSubscriptionCatalog());
  });

  it("Then fees cannot be changed", async () => {
    await renderBackofficeScreen(<PlansScreen />);

    await screen.findByText(translate("backoffice.plans.salesFeeExplanation"));

    expect(
      screen.queryByLabelText(translate("backoffice.plans.changeSalesFeeOf", { plan: "Lite" })),
    ).toBeNull();
  });
});
