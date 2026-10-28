import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import {
  createClassGroup,
  listClassGroupsIncludingInactive,
  listInstructorWeek,
} from "@/features/classGroups/classGroupsApi";
import { ClassGroupFormScreen } from "@/features/classGroups/screens/ClassGroupFormScreen";
import { listActiveInstructors } from "@/features/instructors/instructorsApi";
import { translate } from "@/i18n/translate";
import {
  buildClassGroup,
  buildInstructor,
  buildInstructorWeekSlot,
} from "@/testing/classGroupFactory";
import { fillClassGroupForm, submitClassGroupForm } from "@/testing/fillClassGroupForm";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/classGroups/classGroupsApi");
jest.mock("@/features/instructors/instructorsApi");

describe("When a time free on every day is picked", () => {
  beforeEach(() => {
    jest.mocked(listActiveInstructors).mockResolvedValue([buildInstructor()]);
    jest.mocked(listClassGroupsIncludingInactive).mockResolvedValue([]);
    jest
      .mocked(listInstructorWeek)
      .mockResolvedValue([
        buildInstructorWeekSlot({ weekday: "Monday", startTime: "09:00", durationMinutes: 60 }),
        buildInstructorWeekSlot({ weekday: "Thursday", startTime: "08:00", durationMinutes: 90 }),
      ]);
    jest.mocked(createClassGroup).mockResolvedValue(buildClassGroup());
  });

  it("Then its start time is saved", async () => {
    await renderWithProviders(<ClassGroupFormScreen />);
    await screen.findByText(buildInstructor().fullName);
    await fillClassGroupForm({
      weekdays: ["Monday", "Thursday"],
      startTime: "0930",
      durationMinutes: 60,
    });

    await fireEvent.press(
      await screen.findByRole("button", {
        name: translate("privateLessons.agenda.gapFrom", { time: "10:00" }),
      }),
    );
    await submitClassGroupForm();

    await waitFor(() =>
      expect(createClassGroup).toHaveBeenCalledWith(
        expect.objectContaining({ startTime: "10:00" }),
      ),
    );
  });
});
