import { screen } from "@testing-library/react-native";
import { ApiError } from "@/api/httpClient";
import { classGroupErrorCodes } from "@/features/classGroups/classGroupErrorCodes";
import {
  createClassGroup,
  listClassGroupsIncludingInactive,
} from "@/features/classGroups/classGroupsApi";
import { ClassGroupFormScreen } from "@/features/classGroups/screens/ClassGroupFormScreen";
import { listActiveInstructors } from "@/features/instructors/instructorsApi";
import { formatLongDate } from "@/features/sessions/dates";
import { translate } from "@/i18n/translate";
import { buildInstructor } from "@/testing/classGroupFactory";
import { fillClassGroupForm, submitClassGroupForm } from "@/testing/fillClassGroupForm";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/classGroups/classGroupsApi");
jest.mock("@/features/instructors/instructorsApi");

const conflictStatus = 409;
const privateLessonDate = "2026-10-15";

describe("When instructor has a private lesson at that time", () => {
  beforeEach(() => {
    jest.mocked(listActiveInstructors).mockResolvedValue([buildInstructor()]);
    jest.mocked(listClassGroupsIncludingInactive).mockResolvedValue([]);
    jest.mocked(createClassGroup).mockRejectedValue(
      new ApiError(conflictStatus, {
        code: classGroupErrorCodes.instructorBusy,
        date: privateLessonDate,
        privateLessonId: "lesson",
      }),
    );
  });

  it("Then conflict banner names the date", async () => {
    await renderWithProviders(<ClassGroupFormScreen />);
    await screen.findByText(buildInstructor().fullName);
    await fillClassGroupForm();
    await submitClassGroupForm();

    expect(
      await screen.findByText(
        translate("classGroups.form.instructorBusyPrivateLesson", {
          instructor: buildInstructor().fullName,
          date: formatLongDate(privateLessonDate),
        }),
      ),
    ).toBeOnTheScreen();
  });
});
