import { screen } from "@testing-library/react-native";
import {
  listClassGroupsIncludingInactive,
  listInstructorWeek,
} from "@/features/classGroups/classGroupsApi";
import { ClassGroupFormScreen } from "@/features/classGroups/screens/ClassGroupFormScreen";
import { listActiveInstructors } from "@/features/instructors/instructorsApi";
import { translate } from "@/i18n/translate";
import { buildInstructor, buildInstructorWeekSlot } from "@/testing/classGroupFactory";
import { fillClassGroupForm } from "@/testing/fillClassGroupForm";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/classGroups/classGroupsApi");
jest.mock("@/features/instructors/instructorsApi");

describe("When the start time overlaps a class of the instructor", () => {
  beforeEach(() => {
    jest.mocked(listActiveInstructors).mockResolvedValue([buildInstructor()]);
    jest.mocked(listClassGroupsIncludingInactive).mockResolvedValue([]);
    jest.mocked(listInstructorWeek).mockResolvedValue([
      buildInstructorWeekSlot({
        weekday: "Monday",
        startTime: "09:00",
        durationMinutes: 60,
        classGroupName: "Grupo mañana",
      }),
    ]);
  });

  it("Then the clash is shown on that day before saving", async () => {
    await renderWithProviders(<ClassGroupFormScreen />);
    await screen.findByText(buildInstructor().fullName);

    await fillClassGroupForm({
      weekdays: ["Monday", "Wednesday"],
      startTime: "0930",
      durationMinutes: 60,
    });

    expect(
      await screen.findByText(
        translate("classGroups.week.conflict", {
          name: "Grupo mañana",
          busyStart: "09:00",
          busyEnd: "10:00",
        }),
      ),
    ).toBeOnTheScreen();
    expect(
      screen.getByText(translate("classGroups.week.conflictSummary", { count: 1, total: 2 })),
    ).toBeOnTheScreen();
  });
});
