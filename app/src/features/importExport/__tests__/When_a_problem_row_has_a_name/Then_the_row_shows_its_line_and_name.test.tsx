import { fireEvent, screen } from "@testing-library/react-native";
import { getImportSchema, previewImport } from "@/features/importExport/importExportApi";
import { pickImportFile } from "@/features/importExport/pickImportFile";
import { ImportScreen } from "@/features/importExport/screens/ImportScreen";
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

describe("When a problem row has a name", () => {
  beforeEach(() => {
    jest.mocked(getImportSchema).mockResolvedValue(buildStudentsSchema());
    jest.mocked(pickImportFile).mockResolvedValue(pickedStudentsFile);
    jest.mocked(previewImport).mockResolvedValue(buildStudentsReport());
  });

  it("Then the row shows its line and name", async () => {
    await renderWithProviders(<ImportScreen module="students" />);

    await fireEvent.press(
      screen.getByRole("button", { name: translate("importExport.chooseFile") }),
    );

    expect(
      await screen.findByText(
        translate("importExport.preview.lineWithName", { line: 4, name: "Carla Méndez" }),
      ),
    ).toBeOnTheScreen();
  });
});
