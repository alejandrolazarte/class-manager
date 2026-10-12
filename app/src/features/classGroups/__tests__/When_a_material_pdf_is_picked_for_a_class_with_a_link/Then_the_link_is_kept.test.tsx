import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import {
  listClassGroupsIncludingInactive,
  updateClassGroup,
  uploadClassMaterialFile,
} from "@/features/classGroups/classGroupsApi";
import { pickClassMaterialFile } from "@/features/classGroups/pickClassMaterialFile";
import { ClassGroupFormScreen } from "@/features/classGroups/screens/ClassGroupFormScreen";
import { listActiveInstructors } from "@/features/instructors/instructorsApi";
import { translate } from "@/i18n/translate";
import {
  buildClassGroup,
  buildInstructor,
  buildPickedMaterialFile,
} from "@/testing/classGroupFactory";
import { submitClassGroupForm } from "@/testing/fillClassGroupForm";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/classGroups/classGroupsApi");
jest.mock("@/features/instructors/instructorsApi");
jest.mock("@/features/classGroups/pickClassMaterialFile", () => ({
  ...jest.requireActual("@/features/classGroups/pickClassMaterialFile"),
  pickClassMaterialFile: jest.fn(),
}));

const materialUrl = "https://example.com/video-de-la-clase";
const instructor = buildInstructor();
const pickedFile = buildPickedMaterialFile();
const classGroup = buildClassGroup({ instructorId: instructor.id, materialUrl });

describe("When a material pdf is picked for a class with a link", () => {
  beforeEach(() => {
    jest.mocked(listActiveInstructors).mockResolvedValue([instructor]);
    jest.mocked(listClassGroupsIncludingInactive).mockResolvedValue([classGroup]);
    jest.mocked(pickClassMaterialFile).mockResolvedValue(pickedFile);
    jest.mocked(updateClassGroup).mockResolvedValue(classGroup);
    jest.mocked(uploadClassMaterialFile).mockResolvedValue(classGroup);
  });

  it("Then the link is kept", async () => {
    await renderWithProviders(<ClassGroupFormScreen classGroupId={classGroup.id} />);
    await fireEvent.press(
      await screen.findByRole("button", { name: translate("classGroups.form.uploadMaterialFile") }),
    );
    await screen.findByText(pickedFile.name);
    await submitClassGroupForm();

    await waitFor(() =>
      expect(updateClassGroup).toHaveBeenCalledWith(
        classGroup.id,
        expect.objectContaining({ materialUrl }),
      ),
    );
  });
});
