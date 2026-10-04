import { screen } from "@testing-library/react-native";
import { getStudentAppHome, getStudentAppShop } from "@/features/studentApp/studentAppApi";
import { formatMoney } from "@/features/fees/money";
import { StudentAppHomeScreen } from "@/features/studentApp/screens/StudentAppHomeScreen";
import { addDays, todayIsoDate } from "@/features/sessions/dates";
import { translate } from "@/i18n/translate";
import {
  buildStudentAppHome,
  buildStudentAppNextClass,
  buildStudentAppShop,
  buildAccountStudent,
} from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");

const home = buildStudentAppHome({
  students: [
    buildAccountStudent({
      fullName: "Tomás Pérez",
      nextClasses: [
        buildStudentAppNextClass({
          name: "Natación inicial",
          date: addDays(todayIsoDate(), 1),
          startTime: "18:00",
        }),
      ],
    }),
  ],
});

describe("When student home opens", () => {
  beforeEach(() => {
    jest.mocked(getStudentAppHome).mockResolvedValue(home);
    jest.mocked(getStudentAppShop).mockResolvedValue(buildStudentAppShop());
  });

  it("Then students next classes and fee are shown", async () => {
    await renderStudentAppScreen(<StudentAppHomeScreen />);

    expect(await screen.findByText(home.businessName)).toBeOnTheScreen();
    expect(screen.getByText(translate("student.greeting", { name: "Ana" }))).toBeOnTheScreen();
    expect(screen.getByText(translate("student.nextClass.title"))).toBeOnTheScreen();
    expect(screen.getByText(`${translate("student.day.tomorrow")} · 18:00`)).toBeOnTheScreen();
    expect(screen.getByText("Natación inicial con Laura Gómez")).toBeOnTheScreen();
    expect(screen.getByText(translate("student.fee.title"))).toBeOnTheScreen();
    expect(
      screen.getByText(translate("fees.status.unpaid", { balance: formatMoney(60, "EUR") })),
    ).toBeOnTheScreen();
  });
});
