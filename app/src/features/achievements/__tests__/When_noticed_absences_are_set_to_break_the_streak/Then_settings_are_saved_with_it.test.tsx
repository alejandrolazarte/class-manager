import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import {
  getAchievementSettings,
  updateAchievementSettings,
} from "@/features/achievements/achievementsApi";
import { AchievementSettingsScreen } from "@/features/achievements/screens/AchievementSettingsScreen";
import { translate } from "@/i18n/translate";
import { buildAchievementSettings } from "@/testing/achievementFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/achievements/achievementsApi");

const settings = buildAchievementSettings();

describe("When noticed absences are set to break the streak", () => {
  beforeEach(() => {
    jest.mocked(getAchievementSettings).mockResolvedValue(settings);
    jest.mocked(updateAchievementSettings).mockResolvedValue(settings);
  });

  it("Then settings are saved with it", async () => {
    await renderWithProviders(<AchievementSettingsScreen />);

    await fireEvent.press(
      await screen.findByRole("switch", {
        name: translate("achievements.settings.noticedAbsencesKeepStreak"),
      }),
    );
    await fireEvent.press(screen.getByRole("button", { name: translate("common.save") }));

    await waitFor(() =>
      expect(updateAchievementSettings).toHaveBeenCalledWith(
        expect.objectContaining({ noticedAbsencesKeepStreak: false }),
      ),
    );
  });
});
