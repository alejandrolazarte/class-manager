import { fireEvent, screen } from "@testing-library/react-native";
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

describe("When student opens a plan in the shop", () => {
  beforeEach(() => {
    jest.mocked(getStudentAppShop).mockResolvedValue(
      buildStudentAppShop({
        plans: [buildStudentAppShopPlan()],
        planStudents: [buildStudentAppPlanStudent()],
      }),
    );
  });

  it("Then the plan is not chosen until confirmed", async () => {
    await renderStudentAppScreen(<StudentAppShopScreen />);

    await fireEvent.press(await screen.findByRole("button", { name: "Principiante" }));
    await fireEvent.press(
      screen.getByRole("button", {
        name: `${translate("student.shop.choosePlan")} Tomás Pérez`,
      }),
    );

    expect(
      screen.getByText(
        translate("student.shop.choosePlanQuestion", { plan: "Principiante", name: "Tomás Pérez" }),
      ),
    ).toBeOnTheScreen();
    expect(chooseStudentAppPlan).not.toHaveBeenCalled();
  });
});
