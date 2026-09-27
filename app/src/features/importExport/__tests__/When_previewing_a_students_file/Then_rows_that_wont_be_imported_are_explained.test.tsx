import { fireEvent, screen } from "@testing-library/react-native";
import { getImportSchema, previewImport } from "@/features/importExport/importExportApi";
import { pickImportFile } from "@/features/importExport/pickImportFile";
import { ImportExportScreen } from "@/features/importExport/screens/ImportExportScreen";
import { translate } from "@/i18n/translate";
import {
  buildStudentsReport,
  buildStudentsSchema,
  pickedStudentsFile,
} from "@/testing/importExportFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/importExport/importExportApi");
jest.mock("@/features/importExport/pickImportFile");
jest.mock("@/features/importExport/saveSpreadsheetFile");

describe("When previewing a students file", () => {
  beforeEach(() => {
    jest.mocked(getImportSchema).mockResolvedValue(buildStudentsSchema());
    jest.mocked(pickImportFile).mockResolvedValue(pickedStudentsFile);
    jest.mocked(previewImport).mockResolvedValue(buildStudentsReport());
  });

  it("Then rows that won't be imported are explained", async () => {
    await renderWithProviders(<ImportExportScreen />);

    await fireEvent.press(
      screen.getByRole("button", { name: translate("importExport.chooseFile") }),
    );

    expect(
      await screen.findByText(translate("importExport.rowError.invalid", { column: "Tel" })),
    ).toBeOnTheScreen();
    expect(
      screen.getByText(translate("importExport.rowError.student.already_registered")),
    ).toBeOnTheScreen();
    expect(previewImport).toHaveBeenCalledWith("students", pickedStudentsFile);
  });
});
