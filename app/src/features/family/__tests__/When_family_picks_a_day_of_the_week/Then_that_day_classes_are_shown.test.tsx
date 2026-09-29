import { fireEvent, screen } from "@testing-library/react-native";
import { getFamilyHome } from "@/features/family/familyApi";
import { FamilyClassesScreen } from "@/features/family/screens/FamilyClassesScreen";
import { shortDayLabel } from "@/features/family/familySchedule";
import { addDays, todayIsoDate } from "@/features/sessions/dates";
import { translate } from "@/i18n/translate";
import { buildFamilyHome, buildFamilyNextClass, buildFamilyStudent } from "@/testing/familyFactory";
import { renderFamilyScreen } from "@/testing/renderFamilyScreen";

jest.mock("@/features/family/familyApi");

const today = todayIsoDate();

describe("When family picks a day of the week", () => {
  beforeEach(() => {
    jest.mocked(getFamilyHome).mockResolvedValue(
      buildFamilyHome({
        students: [
          buildFamilyStudent({
            nextClasses: [
              buildFamilyNextClass({ name: "Natación inicial", date: today, startTime: "18:00" }),
              buildFamilyNextClass({
                name: "Natación inicial",
                date: addDays(today, 7),
                startTime: "18:00",
                isCancelled: true,
              }),
            ],
          }),
        ],
      }),
    );
  });

  it("Then that day classes are shown", async () => {
    await renderFamilyScreen(<FamilyClassesScreen />);

    expect(await screen.findByText("18:00–18:45")).toBeOnTheScreen();

    await fireEvent.press(
      screen.getByRole("button", { name: translate("family.schedule.nextWeek") }),
    );
    await fireEvent.press(screen.getByRole("button", { name: shortDayLabel(addDays(today, 7)) }));

    expect(screen.getByText(translate("family.student.cancelled"))).toBeOnTheScreen();
  });
});
