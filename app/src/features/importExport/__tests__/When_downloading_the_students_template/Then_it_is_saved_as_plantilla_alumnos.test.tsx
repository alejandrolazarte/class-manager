import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { downloadTemplate, getImportSchema } from "@/features/importExport/importExportApi";
import { saveCsvFile } from "@/features/importExport/saveCsvFile";
import { ImportExportScreen } from "@/features/importExport/screens/ImportExportScreen";
import { translate } from "@/i18n/translate";
import { buildStudentsSchema } from "@/testing/importExportFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/importExport/importExportApi");
jest.mock("@/features/importExport/pickCsvFile");
jest.mock("@/features/importExport/saveCsvFile");

const templateCsv = "Alumno;Teléfono\r\nLucas Gómez;611 222 333\r\n";

describe("When downloading the students template", () => {
  beforeEach(() => {
    jest.mocked(getImportSchema).mockResolvedValue(buildStudentsSchema());
    jest.mocked(downloadTemplate).mockResolvedValue(templateCsv);
  });

  it("Then it is saved as plantilla alumnos", async () => {
    await renderWithProviders(<ImportExportScreen />);

    await fireEvent.press(
      screen.getByRole("button", { name: translate("importExport.downloadTemplate") }),
    );

    await waitFor(() =>
      expect(saveCsvFile).toHaveBeenCalledWith("plantilla-alumnos.csv", templateCsv),
    );
  });
});
