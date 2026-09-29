import { screen } from "@testing-library/react-native";
import { getFamilyHome } from "@/features/family/familyApi";
import { formatMoney } from "@/features/fees/money";
import { FamilyHomeScreen } from "@/features/family/screens/FamilyHomeScreen";
import { translate } from "@/i18n/translate";
import { buildFamilyHome, buildFamilyNextClass, buildFamilyStudent } from "@/testing/familyFactory";
import { renderWithSession } from "@/testing/renderWithSession";

jest.mock("@/features/family/familyApi");

const home = buildFamilyHome({
  students: [
    buildFamilyStudent({
      fullName: "Tomás Pérez",
      nextClasses: [buildFamilyNextClass({ name: "Natación inicial", startTime: "18:00" })],
    }),
  ],
});

describe("When family home opens", () => {
  beforeEach(() => {
    jest.mocked(getFamilyHome).mockResolvedValue(home);
  });

  it("Then students next classes and fee are shown", async () => {
    await renderWithSession(<FamilyHomeScreen />);

    expect(await screen.findByText(home.businessName)).toBeOnTheScreen();
    expect(screen.getByText("Tomás Pérez")).toBeOnTheScreen();
    expect(screen.getByText("Natación inicial", { exact: false })).toBeOnTheScreen();
    expect(screen.getByText(translate("family.fee.title"))).toBeOnTheScreen();
    expect(
      screen.getByText(translate("fees.status.unpaid", { balance: formatMoney(60, "EUR") })),
    ).toBeOnTheScreen();
  });
});
