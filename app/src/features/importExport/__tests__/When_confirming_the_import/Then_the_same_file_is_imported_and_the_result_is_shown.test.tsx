import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import {
  getImportSchema,
  importFile,
  previewImport,
} from "@/features/importExport/importExportApi";
import { pickCsvFile } from "@/features/importExport/pickCsvFile";
import { ImportExportScreen } from "@/features/importExport/screens/ImportExportScreen";
import { translate, translateCount } from "@/i18n/translate";
import {
  buildStudentsReport,
  buildStudentsSchema,
  pickedStudentsFile,
} from "@/testing/importExportFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/importExport/importExportApi");
jest.mock("@/features/importExport/pickCsvFile");
jest.mock("@/features/importExport/saveCsvFile");

describe("When confirming the import", () => {
  beforeEach(() => {
    jest.mocked(getImportSchema).mockResolvedValue(buildStudentsSchema());
    jest.mocked(pickCsvFile).mockResolvedValue(pickedStudentsFile);
    jest.mocked(previewImport).mockResolvedValue(buildStudentsReport());
    jest.mocked(importFile).mockResolvedValue(buildStudentsReport());
  });

  it("Then the same file is imported and the result is shown", async () => {
    await renderWithProviders(<ImportExportScreen />);
    await fireEvent.press(
      screen.getByRole("button", { name: translate("importExport.chooseFile") }),
    );

    await fireEvent.press(
      await screen.findByRole("button", { name: translateCount("importExport.preview.import", 1) }),
    );

    await waitFor(() => expect(importFile).toHaveBeenCalledWith("students", pickedStudentsFile));
    expect(
      await screen.findByText(translateCount("importExport.result.imported", 1)),
    ).toBeOnTheScreen();
  });
});
