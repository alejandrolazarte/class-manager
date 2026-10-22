import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { updateBusinessSettings } from "@/features/business/businessApi";
import { BusinessSettingsScreen } from "@/features/business/screens/BusinessSettingsScreen";
import { translate } from "@/i18n/translate";
import { buildBusiness } from "@/testing/businessFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/business/businessApi");

const business = buildBusiness();
const locationName = "Av. Costanera 1450, Ciudad";
const locationMapsUrl = "https://maps.app.goo.gl/k3Hq8vPzR2";

describe("When business location is saved", () => {
  beforeEach(() => {
    jest
      .mocked(updateBusinessSettings)
      .mockResolvedValue({ ...business, locationName, locationMapsUrl });
  });

  it("Then request has name and link", async () => {
    await renderWithProviders(<BusinessSettingsScreen />, { business });

    await fireEvent.changeText(
      screen.getByLabelText(translate("businessLocation.link")),
      ` ${locationMapsUrl} `,
    );
    await fireEvent.changeText(
      screen.getByLabelText(translate("businessLocation.name")),
      locationName,
    );
    await fireEvent.press(screen.getByRole("button", { name: translate("common.save") }));

    await waitFor(() =>
      expect(updateBusinessSettings).toHaveBeenCalledWith(
        expect.objectContaining({ locationName, locationMapsUrl }),
      ),
    );
  });
});
