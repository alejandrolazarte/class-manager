import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { ImportExportMenu } from "@/features/importExport/components/ImportExportMenu";
import { downloadExport } from "@/features/importExport/importExportApi";
import { saveSpreadsheetFile } from "@/features/importExport/saveSpreadsheetFile";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/importExport/importExportApi");
jest.mock("@/features/importExport/pickImportFile");
jest.mock("@/features/importExport/saveSpreadsheetFile");

const exportedWorkbook = new Uint8Array([0x50, 0x4b, 0x03, 0x04]).buffer;

describe("When exporting instructors", () => {
  beforeEach(() => {
    jest.mocked(downloadExport).mockResolvedValue(exportedWorkbook);
  });

  it("Then the file is saved with a dated spanish name", async () => {
    await renderWithProviders(
      <ImportExportMenu module="instructors" importRoute={routes.importInstructors} />,
    );

    await fireEvent.press(
      screen.getByRole("button", { name: translate("importExport.moreOptions") }),
    );
    await fireEvent.press(
      screen.getByRole("menuitem", { name: translate("importExport.export.instructors") }),
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
