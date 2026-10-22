import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { updateBusinessSettings } from "@/features/business/businessApi";
import { BusinessSettingsScreen } from "@/features/business/screens/BusinessSettingsScreen";
import { translate } from "@/i18n/translate";
import { buildBusiness } from "@/testing/businessFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/business/businessApi");

const business = buildBusiness();

describe("When business moves to Spain", () => {
  beforeEach(() => {
    jest.mocked(updateBusinessSettings).mockResolvedValue({
      ...business,
      countryCode: "ES",
      timeZoneId: "Europe/Madrid",
      currencyCode: "EUR",
      defaultCountryCallingCode: "34",
    });
  });

  it("Then the country is saved", async () => {
    await renderWithProviders(<BusinessSettingsScreen />, { business });

    await fireEvent.press(await screen.findByRole("button", { name: translate("countries.ES") }));
    await fireEvent.press(screen.getByRole("button", { name: translate("common.save") }));

    await waitFor(() =>
      expect(updateBusinessSettings).toHaveBeenCalledWith({
        name: business.name,
        countryCode: "ES",
        timeZoneId: "Europe/Madrid",
        currencyCode: "EUR",
        locationName: null,
        locationMapsUrl: null,
      }),
    );
  });
});
