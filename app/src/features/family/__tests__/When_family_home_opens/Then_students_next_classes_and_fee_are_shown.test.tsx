import { screen } from "@testing-library/react-native";
import { getFamilyHome, getFamilyShop } from "@/features/family/familyApi";
import { formatMoney } from "@/features/fees/money";
import { FamilyHomeScreen } from "@/features/family/screens/FamilyHomeScreen";
import { addDays, todayIsoDate } from "@/features/sessions/dates";
import { translate } from "@/i18n/translate";
import {
  buildFamilyHome,
  buildFamilyNextClass,
  buildFamilyShop,
  buildFamilyStudent,
} from "@/testing/familyFactory";
import { renderFamilyScreen } from "@/testing/renderFamilyScreen";

jest.mock("@/features/family/familyApi");

const home = buildFamilyHome({
  students: [
    buildFamilyStudent({
      fullName: "Tomás Pérez",
      nextClasses: [
        buildFamilyNextClass({
          name: "Natación inicial",
          date: addDays(todayIsoDate(), 1),
          startTime: "18:00",
        }),
      ],
    }),
  ],
});

describe("When family home opens", () => {
  beforeEach(() => {
    jest.mocked(getFamilyHome).mockResolvedValue(home);
    jest.mocked(getFamilyShop).mockResolvedValue(buildFamilyShop());
  });

  it("Then students next classes and fee are shown", async () => {
    await renderFamilyScreen(<FamilyHomeScreen />);

    expect(await screen.findByText(home.businessName)).toBeOnTheScreen();
    expect(screen.getByText(translate("family.greeting", { name: "Ana" }))).toBeOnTheScreen();
    expect(screen.getByText(translate("family.nextClass.title"))).toBeOnTheScreen();
    expect(screen.getByText(`${translate("family.day.tomorrow")} · 18:00`)).toBeOnTheScreen();
    expect(screen.getByText("Natación inicial con Laura Gómez")).toBeOnTheScreen();
    expect(screen.getByText(translate("family.fee.title"))).toBeOnTheScreen();
    expect(
      screen.getByText(translate("fees.status.unpaid", { balance: formatMoney(60, "EUR") })),
    ).toBeOnTheScreen();
  });
});
