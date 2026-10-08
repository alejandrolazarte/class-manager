import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { createClassGroup } from "@/features/classGroups/classGroupsApi";
import { ClassGroupFormScreen } from "@/features/classGroups/screens/ClassGroupFormScreen";
import { listActiveInstructors } from "@/features/instructors/instructorsApi";
import { translate } from "@/i18n/translate";
import { buildClassGroup, buildInstructor } from "@/testing/classGroupFactory";
import { fillClassGroupForm, submitClassGroupForm } from "@/testing/fillClassGroupForm";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/classGroups/classGroupsApi");
jest.mock("@/features/instructors/instructorsApi");

const instructor = buildInstructor();
const materialUrl = "https://example.com/natacion-adultos.pdf";

describe("When class has material", () => {
  beforeEach(() => {
    jest.mocked(listActiveInstructors).mockResolvedValue([instructor]);
    jest.mocked(createClassGroup).mockResolvedValue(buildClassGroup());
  });

  it("Then the link is sent", async () => {
    await renderWithProviders(<ClassGroupFormScreen />);
    await screen.findByText(instructor.fullName);
    await fillClassGroupForm();
    await fireEvent.changeText(
      screen.getByLabelText(translate("classGroups.form.materialUrl")),
      materialUrl,
    );
    await submitClassGroupForm();

    await waitFor(() =>
      expect(createClassGroup).toHaveBeenCalledWith(expect.objectContaining({ materialUrl })),
    );
  });
});
