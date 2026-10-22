import { fireEvent, screen } from "@testing-library/react-native";
import { Linking } from "react-native";
import { BusinessSettingsScreen } from "@/features/business/screens/BusinessSettingsScreen";
import { translate } from "@/i18n/translate";
import { buildBusiness } from "@/testing/businessFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/business/businessApi");

const locationMapsUrl = "https://maps.app.goo.gl/k3Hq8vPzR2";

describe("When the location link is tried", () => {
  beforeEach(() => {
    jest.spyOn(Linking, "openURL").mockResolvedValue(true);
  });

  it("Then Google Maps opens it", async () => {
    await renderWithProviders(<BusinessSettingsScreen />, {
      business: buildBusiness({ locationName: "Av. Costanera 1450, Ciudad", locationMapsUrl }),
    });

    expect(screen.getByText(translate("businessLocation.linkValid"))).toBeOnTheScreen();
    await fireEvent.press(
      screen.getByRole("button", { name: translate("businessLocation.tryLink") }),
    );

    expect(Linking.openURL).toHaveBeenCalledWith(locationMapsUrl);
  });
});
