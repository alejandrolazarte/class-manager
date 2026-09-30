import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { getFamilyHome, withdrawAbsence } from "@/features/family/familyApi";
import { FamilyClassesScreen } from "@/features/family/screens/FamilyClassesScreen";
import { shortDayLabel } from "@/features/family/familySchedule";
import { addDays, todayIsoDate } from "@/features/sessions/dates";
import { translate } from "@/i18n/translate";
import { buildFamilyHome, buildFamilyNextClass, buildFamilyStudent } from "@/testing/familyFactory";
import { renderFamilyScreen } from "@/testing/renderFamilyScreen";

jest.mock("@/features/family/familyApi");

const nextWeek = addDays(todayIsoDate(), 7);

describe("When family takes back a notice", () => {
  beforeEach(() => {
    jest.mocked(withdrawAbsence).mockResolvedValue();
    jest.mocked(getFamilyHome).mockResolvedValue(
      buildFamilyHome({
        students: [
          buildFamilyStudent({
            id: "student-tomas",
            nextClasses: [
              buildFamilyNextClass({
                date: nextWeek,
                classGroupId: "class-group-1",
                absenceNotified: true,
              }),
            ],
          }),
        ],
      }),
    );
  });

  it("Then the withdrawal is sent", async () => {
    await renderFamilyScreen(<FamilyClassesScreen />);
    await fireEvent.press(
      await screen.findByRole("button", { name: translate("family.schedule.nextWeek") }),
    );
    await fireEvent.press(screen.getByRole("button", { name: shortDayLabel(nextWeek) }));

    expect(screen.getByText(translate("family.absence.status"))).toBeOnTheScreen();
    await fireEvent.press(screen.getByRole("button", { name: translate("family.absence.going") }));

    await waitFor(() =>
      expect(withdrawAbsence).toHaveBeenCalledWith("student-tomas", "class-group-1", nextWeek),
    );
  });
});
