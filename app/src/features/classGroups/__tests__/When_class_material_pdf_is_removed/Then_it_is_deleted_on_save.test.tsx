import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import {
  listClassGroupsIncludingInactive,
  removeClassMaterialFile,
  updateClassGroup,
} from "@/features/classGroups/classGroupsApi";
import { ClassGroupFormScreen } from "@/features/classGroups/screens/ClassGroupFormScreen";
import { listActiveInstructors } from "@/features/instructors/instructorsApi";
import { translate } from "@/i18n/translate";
import { buildClassGroup, buildInstructor } from "@/testing/classGroupFactory";
import { submitClassGroupForm } from "@/testing/fillClassGroupForm";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/classGroups/classGroupsApi");
jest.mock("@/features/instructors/instructorsApi");

const instructor = buildInstructor();
const classGroup = buildClassGroup({
  instructorId: instructor.id,
  materialFile: { id: "file-1", url: "https://files.example.com/guia.pdf", sizeInBytes: 2_400_000 },
});

describe("When class material pdf is removed", () => {
  beforeEach(() => {
    jest.mocked(listActiveInstructors).mockResolvedValue([instructor]);
    jest.mocked(listClassGroupsIncludingInactive).mockResolvedValue([classGroup]);
    jest.mocked(removeClassMaterialFile).mockResolvedValue({ ...classGroup, materialFile: null });
    jest.mocked(updateClassGroup).mockResolvedValue({ ...classGroup, materialFile: null });
  });

  it("Then it is deleted on save", async () => {
    await renderWithProviders(<ClassGroupFormScreen classGroupId={classGroup.id} />);
    await fireEvent.press(
      await screen.findByRole("button", { name: translate("classGroups.form.removeMaterialFile") }),
    );
    expect(removeClassMaterialFile).not.toHaveBeenCalled();
    await submitClassGroupForm();

    await waitFor(() => expect(removeClassMaterialFile).toHaveBeenCalledWith(classGroup.id));
  });
});
