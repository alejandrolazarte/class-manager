import { fireEvent, screen } from "@testing-library/react-native";
import { ApiError } from "@/api/apiErrors";
import { createClassGroup, uploadClassMaterialFile } from "@/features/classGroups/classGroupsApi";
import { pickClassMaterialFile } from "@/features/classGroups/pickClassMaterialFile";
import { ClassGroupFormScreen } from "@/features/classGroups/screens/ClassGroupFormScreen";
import { listActiveInstructors } from "@/features/instructors/instructorsApi";
import { subscriptionErrorCodes } from "@/features/subscriptions/subscriptionCodes";
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

const forbiddenStatus = 403;
const instructor = buildInstructor();
const pickedFile = buildPickedMaterialFile();

describe("When the material pdf goes past the document storage", () => {
  beforeEach(() => {
    jest.mocked(listActiveInstructors).mockResolvedValue([instructor]);
    jest.mocked(pickClassMaterialFile).mockResolvedValue(pickedFile);
    jest.mocked(createClassGroup).mockResolvedValue(buildClassGroup({ id: "new-class" }));
    jest
      .mocked(uploadClassMaterialFile)
      .mockRejectedValue(
        new ApiError(forbiddenStatus, { code: subscriptionErrorCodes.featureLimitReached }),
      );
  });

  it("Then the class is saved with a warning", async () => {
    await renderWithProviders(<ClassGroupFormScreen />);
    await screen.findByText(instructor.fullName);
    await fillClassGroupForm();
    await fireEvent.press(
      screen.getByRole("button", { name: translate("classGroups.form.uploadMaterialFile") }),
    );
    await screen.findByText(pickedFile.name);
    await submitClassGroupForm();

    expect(
      await screen.findByText(translate("classGroups.form.savedWithDocumentStorageFull")),
    ).toBeTruthy();
  });
});
