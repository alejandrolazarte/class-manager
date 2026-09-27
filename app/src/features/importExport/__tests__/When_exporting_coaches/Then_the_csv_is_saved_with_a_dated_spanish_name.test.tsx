import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { downloadExport, getImportSchema } from "@/features/importExport/importExportApi";
import { saveCsvFile } from "@/features/importExport/saveCsvFile";
import { ImportExportScreen } from "@/features/importExport/screens/ImportExportScreen";
import { translate } from "@/i18n/translate";
import { buildInstructorsSchema } from "@/testing/importExportFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/importExport/importExportApi");
jest.mock("@/features/importExport/pickCsvFile");
jest.mock("@/features/importExport/saveCsvFile");

const exportedCsv = "Profesor\r\nMarta Ruiz\r\n";

describe("When exporting coaches", () => {
  beforeEach(() => {
    jest.mocked(getImportSchema).mockResolvedValue(buildInstructorsSchema());
    jest.mocked(downloadExport).mockResolvedValue(exportedCsv);
  });

  it("Then the csv is saved with a dated spanish name", async () => {
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
      expect(saveCsvFile).toHaveBeenCalledWith(
        expect.stringMatching(/^profes-\d{4}-\d{2}-\d{2}\.csv$/),
        exportedCsv,
      ),
    );
    expect(downloadExport).toHaveBeenCalledWith("instructors");
  });
});
