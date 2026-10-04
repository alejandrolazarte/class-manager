import { fireEvent, screen } from "@testing-library/react-native";
import { getStudentAppHome, getStudentAppShop } from "@/features/studentApp/studentAppApi";
import { StudentAppHomeScreen } from "@/features/studentApp/screens/StudentAppHomeScreen";
import { addDays, todayIsoDate } from "@/features/sessions/dates";
import {
  buildStudentAppHome,
  buildStudentAppNextClass,
  buildStudentAppShop,
  buildAccountStudent,
} from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");

describe("When student has two students", () => {
  beforeEach(() => {
    jest.mocked(getStudentAppShop).mockResolvedValue(buildStudentAppShop());
    jest.mocked(getStudentAppHome).mockResolvedValue(
      buildStudentAppHome({
        students: [
          buildAccountStudent({
            id: "student-tomas",
            fullName: "Tomás Pérez",
            nextClasses: [
              buildStudentAppNextClass({ name: "Natación inicial", date: todayIsoDate() }),
            ],
          }),
          buildAccountStudent({
            id: "student-lucia",
            fullName: "Lucía Pérez",
            nextClasses: [
              buildStudentAppNextClass({
                name: "Nivel 3",
                date: addDays(todayIsoDate(), 1),
                instructorFullName: "Caro Díaz",
              }),
            ],
          }),
        ],
      }),
    );
  });

  it("Then the chosen student next class is shown", async () => {
    await renderStudentAppScreen(<StudentAppHomeScreen />);

    await fireEvent.press(await screen.findByRole("button", { name: "Lucía" }));

    expect(screen.getByText("Nivel 3 con Caro Díaz")).toBeOnTheScreen();
    expect(screen.queryByText("Natación inicial con Laura Gómez")).toBeNull();
  });
});
