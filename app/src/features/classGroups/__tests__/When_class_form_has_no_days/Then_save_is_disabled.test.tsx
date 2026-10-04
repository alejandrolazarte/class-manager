import { screen } from "@testing-library/react-native";
import { ClassGroupFormScreen } from "@/features/classGroups/screens/ClassGroupFormScreen";
import { listActiveInstructors } from "@/features/instructors/instructorsApi";
import { translate } from "@/i18n/translate";
import { buildInstructor } from "@/testing/classGroupFactory";
import { fillClassGroupForm } from "@/testing/fillClassGroupForm";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/classGroups/classGroupsApi");
jest.mock("@/features/instructors/instructorsApi");

describe("When class form has no days", () => {
  beforeEach(() => {
    jest.mocked(listActiveInstructors).mockResolvedValue([buildInstructor()]);
  });

  it("Then save is disabled", async () => {
    await renderWithProviders(<ClassGroupFormScreen />);
    await screen.findByRole("button", { name: buildInstructor().fullName });

    await fillClassGroupForm({ weekdays: [] });

    expect(screen.getByRole("button", { name: translate("common.save") })).toBeDisabled();
  });
});
