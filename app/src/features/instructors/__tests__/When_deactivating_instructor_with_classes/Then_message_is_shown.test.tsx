import { fireEvent, screen } from "@testing-library/react-native";
import { ApiError } from "@/api/httpClient";
import { instructorErrorCodes } from "@/features/instructors/instructorErrorCodes";
import {
  listInstructorsIncludingInactive,
  setInstructorActive,
} from "@/features/instructors/instructorsApi";
import { InstructorFormScreen } from "@/features/instructors/screens/InstructorFormScreen";
import { translate } from "@/i18n/translate";
import { buildInstructor } from "@/testing/classGroupFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/instructors/instructorsApi");

const conflictStatus = 409;
const instructor = buildInstructor();

describe("When deactivating instructor with classes", () => {
  beforeEach(() => {
    jest.mocked(listInstructorsIncludingInactive).mockResolvedValue([instructor]);
    jest.mocked(setInstructorActive).mockRejectedValue(
      new ApiError(conflictStatus, {
        code: instructorErrorCodes.hasActiveClassGroups,
        classGroupCount: 2,
      }),
    );
  });

  it("Then message is shown", async () => {
    await renderWithProviders(<InstructorFormScreen instructorId={instructor.id} />);
    await fireEvent.press(
      await screen.findByRole("button", { name: translate("common.deactivate") }),
    );

    expect(
      await screen.findByText(translate("instructors.form.hasActiveClassGroups", { count: 2 })),
    ).toBeOnTheScreen();
  });
});
