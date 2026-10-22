import { fireEvent, screen } from "@testing-library/react-native";
import { BusinessSettingsScreen } from "@/features/business/screens/BusinessSettingsScreen";
import { translate } from "@/i18n/translate";
import { buildBusiness } from "@/testing/businessFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/business/businessApi");

describe("When how to copy the link is opened", () => {
  it("Then the steps are shown", async () => {
    await renderWithProviders(<BusinessSettingsScreen />, { business: buildBusiness() });

    await fireEvent.press(
      screen.getByRole("button", { name: translate("businessLocation.howToCopy") }),
    );

    expect(screen.getByText(translate("businessLocation.help.title"))).toBeOnTheScreen();
    expect(screen.getByText(translate("businessLocation.help.shareTitle"))).toBeOnTheScreen();
    expect(screen.getByText(translate("businessLocation.help.copyTitle"))).toBeOnTheScreen();
  });
});
