import { screen } from "@testing-library/react-native";
import { getFamilyHome } from "@/features/family/familyApi";
import { FamilyProgressScreen } from "@/features/family/screens/FamilyProgressScreen";
import { translate, translateCount } from "@/i18n/translate";
import {
  buildFamilyAttendance,
  buildFamilyHome,
  buildFamilyStudent,
} from "@/testing/familyFactory";
import { renderFamilyScreen } from "@/testing/renderFamilyScreen";

jest.mock("@/features/family/familyApi");

describe("When family opens achievements", () => {
  beforeEach(() => {
    jest.mocked(getFamilyHome).mockResolvedValue(
      buildFamilyHome({
        levels: [
          { name: "Inicial", requiredClasses: 0 },
          { name: "Base", requiredClasses: 10 },
          { name: "Explorador", requiredClasses: 25 },
        ],
        students: [
          buildFamilyStudent({
            attendance: buildFamilyAttendance({
              attendedClasses: 18,
              level: 2,
              medals: ["FirstClass", "TenClasses", "LeveledUp"],
            }),
          }),
        ],
      }),
    );
  });

  it("Then level and medals are shown", async () => {
    await renderFamilyScreen(<FamilyProgressScreen />);

    expect(
      await screen.findByText(translate("family.progress.level", { number: 2, name: "Base" })),
    ).toBeOnTheScreen();
    expect(
      screen.getByText(translateCount("family.progress.classesToNext", 7, { name: "Explorador" })),
    ).toBeOnTheScreen();
    expect(
      screen.getByLabelText(
        translate("family.progress.medalEarned", { name: translate("family.medals.TenClasses") }),
      ),
    ).toBeOnTheScreen();
    expect(
      screen.getByLabelText(
        translate("family.progress.medalLocked", {
          name: translate("family.medals.HundredClasses"),
        }),
      ),
    ).toBeOnTheScreen();
  });
});
