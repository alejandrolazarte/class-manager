import { screen } from "@testing-library/react-native";
import { createClassGroup } from "@/features/classGroups/classGroupsApi";
import { ClassGroupFormScreen } from "@/features/classGroups/screens/ClassGroupFormScreen";
import { listActiveInstructors } from "@/features/instructors/instructorsApi";
import { translate } from "@/i18n/translate";
import { buildInstructor } from "@/testing/classGroupFactory";
import { fillClassGroupForm, submitClassGroupForm } from "@/testing/fillClassGroupForm";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/classGroups/classGroupsApi");
jest.mock("@/features/instructors/instructorsApi");

describe("When class form is submitted without days", () => {
  beforeEach(() => {
    jest.mocked(listActiveInstructors).mockResolvedValue([buildInstructor()]);
  });

  it("Then days error is shown", async () => {
    await renderWithProviders(<ClassGroupFormScreen />);
    await screen.findByRole("button", { name: buildInstructor().fullName });
    await fillClassGroupForm({ weekdays: [] });
    await submitClassGroupForm();

    expect(
      await screen.findByText(translate("classGroups.validation.weekdaysRequired")),
    ).toBeOnTheScreen();
    expect(createClassGroup).not.toHaveBeenCalled();
  });
});
