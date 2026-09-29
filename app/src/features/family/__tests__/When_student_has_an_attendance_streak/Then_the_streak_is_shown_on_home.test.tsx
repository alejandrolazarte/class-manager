import { screen } from "@testing-library/react-native";
import { getFamilyHome, getFamilyShop } from "@/features/family/familyApi";
import { FamilyHomeScreen } from "@/features/family/screens/FamilyHomeScreen";
import { translate, translateCount } from "@/i18n/translate";
import {
  buildFamilyAttendance,
  buildFamilyHome,
  buildFamilyShop,
  buildFamilyStudent,
} from "@/testing/familyFactory";
import { renderFamilyScreen } from "@/testing/renderFamilyScreen";

jest.mock("@/features/family/familyApi");

describe("When student has an attendance streak", () => {
  beforeEach(() => {
    jest.mocked(getFamilyShop).mockResolvedValue(buildFamilyShop());
    jest.mocked(getFamilyHome).mockResolvedValue(
      buildFamilyHome({
        students: [
          buildFamilyStudent({
            attendance: buildFamilyAttendance({ streakWeeks: 6, streakSince: "2026-08-10" }),
          }),
        ],
      }),
    );
  });

  it("Then the streak is shown on home", async () => {
    await renderFamilyScreen(<FamilyHomeScreen />);

    expect(await screen.findByText(translateCount("family.streak.weeks", 6))).toBeOnTheScreen();
    expect(
      screen.getByText(translate("family.streak.since", { month: "agosto" })),
    ).toBeOnTheScreen();
  });
});
