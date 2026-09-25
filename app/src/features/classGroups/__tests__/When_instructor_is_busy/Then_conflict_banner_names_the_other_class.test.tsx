import { screen } from "@testing-library/react-native";
import { ApiError } from "@/api/httpClient";
import { classGroupErrorCodes } from "@/features/classGroups/classGroupErrorCodes";
import {
  createClassGroup,
  listClassGroupsIncludingInactive,
} from "@/features/classGroups/classGroupsApi";
import { ClassGroupFormScreen } from "@/features/classGroups/screens/ClassGroupFormScreen";
import { listActiveInstructors } from "@/features/instructors/instructorsApi";
import { translate } from "@/i18n/translate";
import { buildClassGroup, buildInstructor } from "@/testing/classGroupFactory";
import { fillClassGroupForm, submitClassGroupForm } from "@/testing/fillClassGroupForm";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/classGroups/classGroupsApi");
jest.mock("@/features/instructors/instructorsApi");

const conflictStatus = 409;
const otherClassGroup = buildClassGroup({ id: "other", name: "Aquagym", startTime: "18:30" });

describe("When instructor is busy", () => {
  beforeEach(() => {
    jest.mocked(listActiveInstructors).mockResolvedValue([buildInstructor()]);
    jest.mocked(listClassGroupsIncludingInactive).mockResolvedValue([otherClassGroup]);
    jest.mocked(createClassGroup).mockRejectedValue(
      new ApiError(conflictStatus, {
        code: classGroupErrorCodes.instructorBusy,
        classGroupId: otherClassGroup.id,
      }),
    );
  });

  it("Then conflict banner names the other class", async () => {
    await renderWithProviders(<ClassGroupFormScreen />);
    await screen.findByRole("button", { name: buildInstructor().fullName });
    await fillClassGroupForm();
    await submitClassGroupForm();

    expect(
      await screen.findByText(
        translate("classGroups.form.instructorBusy", {
          instructor: otherClassGroup.instructorFullName,
          name: otherClassGroup.name,
          startTime: otherClassGroup.startTime,
        }),
      ),
    ).toBeOnTheScreen();
  });
});
