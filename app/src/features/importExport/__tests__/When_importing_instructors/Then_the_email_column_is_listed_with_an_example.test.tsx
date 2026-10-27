import { screen } from "@testing-library/react-native";
import { getImportSchema } from "@/features/importExport/importExportApi";
import { ImportScreen } from "@/features/importExport/screens/ImportScreen";
import { translate } from "@/i18n/translate";
import { buildInstructorsSchema } from "@/testing/importExportFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/importExport/importExportApi");
jest.mock("@/features/importExport/pickImportFile");
jest.mock("@/features/importExport/saveSpreadsheetFile");

describe("When importing instructors", () => {
  beforeEach(() => {
    jest.mocked(getImportSchema).mockResolvedValue(buildInstructorsSchema());
  });

  it("Then the email column is listed with an example", async () => {
    await renderWithProviders(<ImportScreen module="instructors" />);

    expect(await screen.findByText("Email")).toBeOnTheScreen();
    expect(
      screen.getByText(translate("importExport.column.example", { example: "laura@example.com" })),
    ).toBeOnTheScreen();
  });
});
