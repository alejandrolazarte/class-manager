import { fireEvent, screen } from "@testing-library/react-native";
import { getStudentAppShop } from "@/features/studentApp/studentAppApi";
import { StudentAppShopScreen } from "@/features/studentApp/screens/StudentAppShopScreen";
import { translate } from "@/i18n/translate";
import { buildStudentAppShop } from "@/testing/studentAppFactory";
import {
  buildStudentAppPlanStudent,
  buildStudentAppShopPlan,
} from "@/testing/studentAppPlanFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");

describe("When a minor opens a plan", () => {
  beforeEach(() => {
    jest.mocked(getStudentAppShop).mockResolvedValue(
      buildStudentAppShop({
        canBuy: false,
        plans: [buildStudentAppShopPlan()],
        planStudents: [buildStudentAppPlanStudent()],
      }),
    );
  });

  it("Then the plan cannot be chosen", async () => {
    await renderStudentAppScreen(<StudentAppShopScreen />);

    await fireEvent.press(await screen.findByRole("button", { name: "Principiante" }));

    expect(
      screen.queryByRole("button", { name: `${translate("student.shop.choosePlan")} Tomás Pérez` }),
    ).toBeNull();
  });
});
