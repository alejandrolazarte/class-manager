import { fireEvent, screen } from "@testing-library/react-native";
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

describe("When levels do not increase", () => {
  beforeEach(() => {
    jest.mocked(getAchievementSettings).mockResolvedValue(settings);
  });

  it("Then nothing is saved", async () => {
    await renderWithProviders(<AchievementSettingsScreen />);

    await fireEvent.changeText(
      await screen.findByLabelText(translate("achievements.settings.levelClasses", { number: 3 })),
      "5",
    );
    await fireEvent.press(screen.getByRole("button", { name: translate("common.save") }));

    expect(
      await screen.findByText(translate("achievements.settings.errors.classesMustIncrease")),
    ).toBeOnTheScreen();
    expect(updateAchievementSettings).not.toHaveBeenCalled();
  });
});
