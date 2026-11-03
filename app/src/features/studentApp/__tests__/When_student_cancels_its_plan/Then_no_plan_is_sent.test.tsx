import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { chooseStudentAppPlan, getStudentAppShop } from "@/features/studentApp/studentAppApi";
import { StudentAppShopScreen } from "@/features/studentApp/screens/StudentAppShopScreen";
import { translate } from "@/i18n/translate";
import { buildStudentAppShop } from "@/testing/studentAppFactory";
import {
  buildStudentAppPlanStudent,
  buildStudentAppShopPlan,
} from "@/testing/studentAppPlanFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");

describe("When student cancels its plan", () => {
  beforeEach(() => {
    jest.mocked(getStudentAppShop).mockResolvedValue(
      buildStudentAppShop({
        plans: [buildStudentAppShopPlan()],
        planStudents: [
          buildStudentAppPlanStudent({
            currentPlanId: "plan-monthly",
            currentPlanName: "Principiante",
            nextMonthPlanId: "plan-monthly",
          }),
        ],
      }),
    );
    jest.mocked(chooseStudentAppPlan).mockResolvedValue();
  });

  it("Then no plan is sent", async () => {
    await renderStudentAppScreen(<StudentAppShopScreen />);
    await fireEvent.press(await screen.findByRole("button", { name: "Principiante" }));
    await fireEvent.press(
      screen.getByRole("button", {
        name: `${translate("student.shop.cancelPlan")} Tomás Pérez`,
      }),
    );

    const confirmButtons = screen.getAllByRole("button", {
      name: translate("student.shop.cancelPlan"),
    });
    await fireEvent.press(confirmButtons[confirmButtons.length - 1]);

    await waitFor(() => expect(chooseStudentAppPlan).toHaveBeenCalledWith("student-1", null));
  });
});
