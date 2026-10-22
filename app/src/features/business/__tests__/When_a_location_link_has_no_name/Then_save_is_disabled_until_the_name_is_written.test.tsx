import { fireEvent, screen } from "@testing-library/react-native";
import { BusinessSettingsScreen } from "@/features/business/screens/BusinessSettingsScreen";
import { translate } from "@/i18n/translate";
import { buildBusiness } from "@/testing/businessFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/business/businessApi");

describe("When a location link has no name", () => {
  it("Then save is disabled until the name is written", async () => {
    await renderWithProviders(<BusinessSettingsScreen />, { business: buildBusiness() });

    await fireEvent.changeText(
      screen.getByLabelText(translate("businessLocation.link")),
      "https://maps.app.goo.gl/k3Hq8vPzR2",
    );

    expect(screen.getByRole("button", { name: translate("common.save") })).toBeDisabled();
    await fireEvent.changeText(
      screen.getByLabelText(translate("businessLocation.name")),
      "Av. Costanera 1450, Ciudad",
    );
    expect(screen.getByRole("button", { name: translate("common.save") })).toBeEnabled();
  });
});
