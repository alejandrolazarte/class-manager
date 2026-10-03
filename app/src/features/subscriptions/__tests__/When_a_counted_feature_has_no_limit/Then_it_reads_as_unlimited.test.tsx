import { screen } from "@testing-library/react-native";
import { FeatureList } from "@/features/subscriptions/components/FeatureList";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";

describe("When a counted feature has no limit", () => {
  it("Then it reads as unlimited", async () => {
    await renderWithProviders(<FeatureList features={[{ code: "students", limit: null }]} />);

    expect(
      screen.getByText(
        translate("subscriptions.featureUnlimited", {
          feature: translate("subscriptions.feature.students"),
        }),
      ),
    ).toBeTruthy();
  });
});
