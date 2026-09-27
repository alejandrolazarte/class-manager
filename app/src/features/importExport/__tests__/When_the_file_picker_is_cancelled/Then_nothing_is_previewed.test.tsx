import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { getImportSchema, previewImport } from "@/features/importExport/importExportApi";
import { pickCsvFile } from "@/features/importExport/pickCsvFile";
import { ImportExportScreen } from "@/features/importExport/screens/ImportExportScreen";
import { translate } from "@/i18n/translate";
import { buildStudentsSchema } from "@/testing/importExportFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/importExport/importExportApi");
jest.mock("@/features/importExport/pickCsvFile");
jest.mock("@/features/importExport/saveCsvFile");

describe("When the file picker is cancelled", () => {
  beforeEach(() => {
    jest.mocked(getImportSchema).mockResolvedValue(buildStudentsSchema());
    jest.mocked(pickCsvFile).mockResolvedValue(null);
  });

  it("Then nothing is previewed", async () => {
    await renderWithProviders(<ImportExportScreen />);

    await fireEvent.press(
      screen.getByRole("button", { name: translate("importExport.chooseFile") }),
    );

    await waitFor(() => expect(pickCsvFile).toHaveBeenCalled());
    expect(previewImport).not.toHaveBeenCalled();
  });
});
