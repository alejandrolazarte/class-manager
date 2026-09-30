import { screen } from "@testing-library/react-native";
import { getFamilyHome, getFamilyMakeups } from "@/features/family/familyApi";
import { FamilyClassesScreen } from "@/features/family/screens/FamilyClassesScreen";
import { todayIsoDate } from "@/features/sessions/dates";
import { translate } from "@/i18n/translate";
import {
  buildFamilyHome,
  buildFamilyMakeups,
  buildFamilyMakeupSlot,
  buildFamilyStudent,
} from "@/testing/familyFactory";
import { renderFamilyScreen } from "@/testing/renderFamilyScreen";

jest.mock("@/features/family/familyApi");

const today = todayIsoDate();

describe("When family has no makeup credits", () => {
  beforeEach(() => {
    jest
      .mocked(getFamilyHome)
      .mockResolvedValue(buildFamilyHome({ students: [buildFamilyStudent({ nextClasses: [] })] }));
    jest
      .mocked(getFamilyMakeups)
      .mockResolvedValue(
        buildFamilyMakeups({ credits: [], slots: [buildFamilyMakeupSlot({ date: today })] }),
      );
  });

  it("Then no class is offered", async () => {
    await renderFamilyScreen(<FamilyClassesScreen />);

    expect(await screen.findByText(translate("family.makeup.none"))).toBeOnTheScreen();
    expect(screen.queryByRole("button", { name: translate("family.makeup.book") })).toBeNull();
  });
});
