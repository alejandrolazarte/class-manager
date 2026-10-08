import { fireEvent, screen } from "@testing-library/react-native";
import {
  classMaterialMaximumSizeInBytes,
  pickClassMaterialFile,
} from "@/features/classGroups/pickClassMaterialFile";
import { ClassGroupFormScreen } from "@/features/classGroups/screens/ClassGroupFormScreen";
import { listActiveInstructors } from "@/features/instructors/instructorsApi";
import { translate } from "@/i18n/translate";
import { buildInstructor, buildPickedMaterialFile } from "@/testing/classGroupFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/classGroups/classGroupsApi");
jest.mock("@/features/instructors/instructorsApi");
jest.mock("@/features/classGroups/pickClassMaterialFile", () => ({
  ...jest.requireActual("@/features/classGroups/pickClassMaterialFile"),
  pickClassMaterialFile: jest.fn(),
}));

const instructor = buildInstructor();
const largeFile = buildPickedMaterialFile({ size: classMaterialMaximumSizeInBytes + 1 });

describe("When class material pdf is too large", () => {
  beforeEach(() => {
    jest.mocked(listActiveInstructors).mockResolvedValue([instructor]);
    jest.mocked(pickClassMaterialFile).mockResolvedValue(largeFile);
  });

  it("Then it is not attached", async () => {
    await renderWithProviders(<ClassGroupFormScreen />);
    await screen.findByText(instructor.fullName);
    await fireEvent.press(
      screen.getByRole("button", { name: translate("classGroups.form.uploadMaterialFile") }),
    );

    expect(
      await screen.findByText(translate("classGroups.validation.materialFileTooLarge")),
    ).toBeOnTheScreen();
    expect(screen.queryByText(largeFile.name)).toBeNull();
  });
});
