import { screen } from "@testing-library/react-native";
import { legalDocuments } from "@/features/legal/legalDocuments";
import { LegalDocumentScreen } from "@/features/legal/screens/LegalDocumentScreen";
import { translate } from "@/i18n/translate";
import { searchParametersMock } from "@/testing/expoRouterMock";
import { renderWithProviders } from "@/testing/renderWithProviders";

describe("When a legal document is opened", () => {
  beforeEach(() => {
    searchParametersMock.current = { kind: "privacy" };
  });

  it("Then its sections are shown", async () => {
    await renderWithProviders(<LegalDocumentScreen />);

    expect(screen.getByText(translate("legal.privacy.title"))).toBeOnTheScreen();
    for (const section of legalDocuments.privacy.sections) {
      expect(screen.getByText(section.heading)).toBeOnTheScreen();
    }
  });
});
