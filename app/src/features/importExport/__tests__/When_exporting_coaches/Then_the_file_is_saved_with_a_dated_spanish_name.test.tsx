import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { downloadExport, getImportSchema } from "@/features/importExport/importExportApi";
import { saveSpreadsheetFile } from "@/features/importExport/saveSpreadsheetFile";
import { ImportExportScreen } from "@/features/importExport/screens/ImportExportScreen";
import { translate } from "@/i18n/translate";
import { buildInstructorsSchema } from "@/testing/importExportFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/importExport/importExportApi");
jest.mock("@/features/importExport/pickImportFile");
jest.mock("@/features/importExport/saveSpreadsheetFile");

const exportedWorkbook = new Uint8Array([0x50, 0x4b, 0x03, 0x04]).buffer;

describe("When exporting coaches", () => {
  beforeEach(() => {
    jest.mocked(getImportSchema).mockResolvedValue(buildInstructorsSchema());
    jest.mocked(downloadExport).mockResolvedValue(exportedWorkbook);
  });

  it("Then the file is saved with a dated spanish name", async () => {
    await renderWithProviders(<ImportExportScreen />);

    await fireEvent.press(
      screen.getByRole("button", { name: translate("importExport.module.instructors") }),
    );
    await fireEvent.press(
      screen.getByRole("button", {
        name: translate("importExport.export", {
          module: translate("importExport.fileModule.instructors"),
        }),
      }),
    );

    await waitFor(() =>
      expect(saveSpreadsheetFile).toHaveBeenCalledWith(
        expect.stringMatching(/^profes-\d{4}-\d{2}-\d{2}\.xlsx$/),
        exportedWorkbook,
      ),
    );
    expect(downloadExport).toHaveBeenCalledWith("instructors");
  });
});
