import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { updateBusinessSettings } from "@/features/business/businessApi";
import { BusinessSettingsScreen } from "@/features/business/screens/BusinessSettingsScreen";
import { translate } from "@/i18n/translate";
import { buildBusiness } from "@/testing/businessFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/business/businessApi");

const business = buildBusiness();

describe("When noticed absences are set to break the streak", () => {
  beforeEach(() => {
    jest
      .mocked(updateBusinessSettings)
      .mockResolvedValue({ ...business, noticedAbsencesKeepStreak: false });
  });

  it("Then settings are saved with it", async () => {
    await renderWithProviders(<BusinessSettingsScreen />, { business });

    await fireEvent.press(
      screen.getByRole("switch", { name: translate("businessSettings.noticedAbsencesKeepStreak") }),
    );
    await fireEvent.press(screen.getByRole("button", { name: translate("common.save") }));

    await waitFor(() =>
      expect(updateBusinessSettings).toHaveBeenCalledWith(
        expect.objectContaining({ noticedAbsencesKeepStreak: false }),
      ),
    );
  });
});
