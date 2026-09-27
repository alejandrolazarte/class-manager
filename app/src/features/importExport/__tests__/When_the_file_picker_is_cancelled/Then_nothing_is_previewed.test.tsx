import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { getImportSchema, previewImport } from "@/features/importExport/importExportApi";
import { pickImportFile } from "@/features/importExport/pickImportFile";
import { ImportExportScreen } from "@/features/importExport/screens/ImportExportScreen";
import { translate } from "@/i18n/translate";
import { buildStudentsSchema } from "@/testing/importExportFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/importExport/importExportApi");
jest.mock("@/features/importExport/pickImportFile");
jest.mock("@/features/importExport/saveSpreadsheetFile");

describe("When the file picker is cancelled", () => {
  beforeEach(() => {
    jest.mocked(getImportSchema).mockResolvedValue(buildStudentsSchema());
    jest.mocked(pickImportFile).mockResolvedValue(null);
  });

  it("Then nothing is previewed", async () => {
    await renderWithProviders(<ImportExportScreen />);

    await fireEvent.press(
      screen.getByRole("button", { name: translate("importExport.chooseFile") }),
    );

    await waitFor(() => expect(pickImportFile).toHaveBeenCalled());
    expect(previewImport).not.toHaveBeenCalled();
  });
});
