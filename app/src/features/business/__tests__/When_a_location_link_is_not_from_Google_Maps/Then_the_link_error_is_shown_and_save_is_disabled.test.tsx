import { fireEvent, screen } from "@testing-library/react-native";
import { BusinessSettingsScreen } from "@/features/business/screens/BusinessSettingsScreen";
import { translate } from "@/i18n/translate";
import { buildBusiness } from "@/testing/businessFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/business/businessApi");

describe("When a location link is not from Google Maps", () => {
  it("Then the link error is shown and save is disabled", async () => {
    await renderWithProviders(<BusinessSettingsScreen />, {
      business: buildBusiness({ locationName: "Av. Costanera 1450, Ciudad" }),
    });
    const linkField = screen.getByLabelText(translate("businessLocation.link"));

    await fireEvent.changeText(linkField, "www.escueladelparque.com");
    await fireEvent(linkField, "blur");

    expect(screen.getByText(translate("businessLocation.linkInvalid"))).toBeOnTheScreen();
    expect(screen.getByRole("button", { name: translate("common.save") })).toBeDisabled();
  });
});
