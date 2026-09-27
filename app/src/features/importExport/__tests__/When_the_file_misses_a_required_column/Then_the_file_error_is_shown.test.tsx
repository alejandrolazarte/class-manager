import { fireEvent, screen } from "@testing-library/react-native";
import { ApiError } from "@/api/apiErrors";
import { getImportSchema, previewImport } from "@/features/importExport/importExportApi";
import { pickCsvFile } from "@/features/importExport/pickCsvFile";
import { ImportExportScreen } from "@/features/importExport/screens/ImportExportScreen";
import { translate } from "@/i18n/translate";
import { buildStudentsSchema, pickedStudentsFile } from "@/testing/importExportFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/importExport/importExportApi");
jest.mock("@/features/importExport/pickCsvFile");
jest.mock("@/features/importExport/saveCsvFile");

const badRequestStatus = 400;

describe("When the file misses a required column", () => {
  beforeEach(() => {
    jest.mocked(getImportSchema).mockResolvedValue(buildStudentsSchema());
    jest.mocked(pickCsvFile).mockResolvedValue(pickedStudentsFile);
    jest.mocked(previewImport).mockRejectedValue(
      new ApiError(badRequestStatus, {
        status: badRequestStatus,
        code: "import.missing_columns",
      }),
    );
  });

  it("Then the file error is shown", async () => {
    await renderWithProviders(<ImportExportScreen />);

    await fireEvent.press(
      screen.getByRole("button", { name: translate("importExport.chooseFile") }),
    );

    expect(
      await screen.findByText(translate("importExport.fileError.import.missing_columns")),
    ).toBeOnTheScreen();
  });
});
