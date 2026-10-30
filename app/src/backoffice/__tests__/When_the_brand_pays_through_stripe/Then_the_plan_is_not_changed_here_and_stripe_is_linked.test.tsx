import { screen } from "@testing-library/react-native";
import { BrandScreen } from "@/backoffice/brands/BrandScreen";
import { getBrand } from "@/backoffice/brands/brandsApi";
import { translate } from "@/i18n/translate";
import { aBrandDetail } from "@/testing/backofficeBrands";
import { signInToBackofficeAs } from "@/testing/backofficeSession";
import { renderBackofficeScreen } from "@/testing/renderBackofficeScreen";

jest.mock("@/backoffice/brands/brandsApi");
jest.mock("@/backoffice/session/backofficeSessionApi");

const brand = aBrandDetail({ billingCustomerUrl: "https://dashboard.stripe.com/customers/cus_1" });

describe("When the brand pays through Stripe", () => {
  beforeEach(() => {
    signInToBackofficeAs("PlatformOperator");
    jest.mocked(getBrand).mockResolvedValue({
      ...brand,
      subscription: { ...brand.subscription!, isExternal: true, paymentStatus: "PastDue" },
    });
  });

  it("Then the plan is not changed here and Stripe is linked", async () => {
    await renderBackofficeScreen(<BrandScreen brandId="brand-1" />);

    expect(await screen.findByText(translate("backoffice.brand.stripePastDue"))).toBeTruthy();
    expect(screen.getByText(translate("backoffice.brand.openInStripe"))).toBeTruthy();
    expect(screen.getByText(translate("backoffice.brand.planChangesInStripe"))).toBeTruthy();
    expect(
      screen.queryByRole("button", { name: translate("backoffice.brand.changePlan") }),
    ).not.toBeOnTheScreen();
    expect(
      screen.queryByRole("button", { name: translate("backoffice.brand.offerPlan") }),
    ).not.toBeOnTheScreen();
  });
});
