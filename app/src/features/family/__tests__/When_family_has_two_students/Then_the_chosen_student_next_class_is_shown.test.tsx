import { fireEvent, screen } from "@testing-library/react-native";
import { getFamilyHome, getFamilyShop } from "@/features/family/familyApi";
import { FamilyHomeScreen } from "@/features/family/screens/FamilyHomeScreen";
import { addDays, todayIsoDate } from "@/features/sessions/dates";
import {
  buildFamilyHome,
  buildFamilyNextClass,
  buildFamilyShop,
  buildFamilyStudent,
} from "@/testing/familyFactory";
import { renderFamilyScreen } from "@/testing/renderFamilyScreen";

jest.mock("@/features/family/familyApi");

describe("When family has two students", () => {
  beforeEach(() => {
    jest.mocked(getFamilyShop).mockResolvedValue(buildFamilyShop());
    jest.mocked(getFamilyHome).mockResolvedValue(
      buildFamilyHome({
        students: [
          buildFamilyStudent({
            id: "student-tomas",
            fullName: "Tomás Pérez",
            nextClasses: [buildFamilyNextClass({ name: "Natación inicial", date: todayIsoDate() })],
          }),
          buildFamilyStudent({
            id: "student-lucia",
            fullName: "Lucía Pérez",
            nextClasses: [
              buildFamilyNextClass({
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
    await renderFamilyScreen(<FamilyHomeScreen />);

    await fireEvent.press(await screen.findByRole("button", { name: "Lucía" }));

    expect(screen.getByText("Nivel 3 con Caro Díaz")).toBeOnTheScreen();
    expect(screen.queryByText("Natación inicial con Laura Gómez")).toBeNull();
  });
});
