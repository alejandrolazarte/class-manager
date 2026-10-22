import { fireEvent, screen } from "@testing-library/react-native";
import { getBrand, updateBrand } from "@/features/brand/brandApi";
import { BrandProvider } from "@/features/brand/BrandProvider";
import { BrandSettingsScreen } from "@/features/brand/screens/BrandSettingsScreen";
import { translate } from "@/i18n/translate";
import { buildBrand } from "@/testing/brandFactory";
import { buildCurrentMember } from "@/testing/memberFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildSubscription } from "@/testing/subscriptionFactory";

jest.mock("@/features/brand/brandApi");

describe("When the plan does not include the brand", () => {
  beforeEach(() => {
    jest.mocked(getBrand).mockResolvedValue(buildBrand({ brandName: null }));
  });

  it("Then saving offers a plan", async () => {
    await renderWithProviders(
      <BrandProvider audience="team">
        <BrandSettingsScreen />
      </BrandProvider>,
      { member: buildCurrentMember({ subscription: buildSubscription() }) },
    );

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("brand.settings.save") }),
    );

    expect(screen.getByText(translate("subscriptions.locked.brand"))).toBeTruthy();
    expect(updateBrand).not.toHaveBeenCalled();
  });
});
