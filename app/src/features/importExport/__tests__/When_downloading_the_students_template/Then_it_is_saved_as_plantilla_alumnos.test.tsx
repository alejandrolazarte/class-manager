import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { downloadTemplate, getImportSchema } from "@/features/importExport/importExportApi";
import { saveSpreadsheetFile } from "@/features/importExport/saveSpreadsheetFile";
import { ImportExportScreen } from "@/features/importExport/screens/ImportExportScreen";
import { translate } from "@/i18n/translate";
import { buildStudentsSchema } from "@/testing/importExportFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/importExport/importExportApi");
jest.mock("@/features/importExport/pickImportFile");
jest.mock("@/features/importExport/saveSpreadsheetFile");

const templateWorkbook = new Uint8Array([0x50, 0x4b, 0x03, 0x04]).buffer;

describe("When downloading the students template", () => {
  beforeEach(() => {
    jest.mocked(getImportSchema).mockResolvedValue(buildStudentsSchema());
    jest.mocked(downloadTemplate).mockResolvedValue(templateWorkbook);
  });

  it("Then it is saved as plantilla alumnos", async () => {
    await renderWithProviders(<ImportExportScreen />);

    await fireEvent.press(
      screen.getByRole("button", { name: translate("importExport.downloadTemplate") }),
    );

    await waitFor(() =>
      expect(saveSpreadsheetFile).toHaveBeenCalledWith("plantilla-alumnos.xlsx", templateWorkbook),
    );
  });
});
