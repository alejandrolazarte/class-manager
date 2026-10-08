import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { createClassGroup, uploadClassMaterialFile } from "@/features/classGroups/classGroupsApi";
import { pickClassMaterialFile } from "@/features/classGroups/pickClassMaterialFile";
import { ClassGroupFormScreen } from "@/features/classGroups/screens/ClassGroupFormScreen";
import { listActiveInstructors } from "@/features/instructors/instructorsApi";
import { translate } from "@/i18n/translate";
import {
  buildClassGroup,
  buildInstructor,
  buildPickedMaterialFile,
} from "@/testing/classGroupFactory";
import { fillClassGroupForm, submitClassGroupForm } from "@/testing/fillClassGroupForm";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/classGroups/classGroupsApi");
jest.mock("@/features/instructors/instructorsApi");
jest.mock("@/features/classGroups/pickClassMaterialFile", () => ({
  ...jest.requireActual("@/features/classGroups/pickClassMaterialFile"),
  pickClassMaterialFile: jest.fn(),
}));

const instructor = buildInstructor();
const pickedFile = buildPickedMaterialFile();
const createdClassGroup = buildClassGroup({ id: "new-class" });

describe("When class is created with a material pdf", () => {
  beforeEach(() => {
    jest.mocked(listActiveInstructors).mockResolvedValue([instructor]);
    jest.mocked(pickClassMaterialFile).mockResolvedValue(pickedFile);
    jest.mocked(createClassGroup).mockResolvedValue(createdClassGroup);
    jest.mocked(uploadClassMaterialFile).mockResolvedValue(createdClassGroup);
  });

  it("Then the pdf is uploaded to the new class", async () => {
    await renderWithProviders(<ClassGroupFormScreen />);
    await screen.findByRole("button", { name: instructor.fullName });
    await fillClassGroupForm();
    await fireEvent.press(
      screen.getByRole("button", { name: translate("classGroups.form.uploadMaterialFile") }),
    );
    await screen.findByText(pickedFile.name);
    await submitClassGroupForm();

    await waitFor(() =>
      expect(uploadClassMaterialFile).toHaveBeenCalledWith("new-class", pickedFile),
    );
  });
});
