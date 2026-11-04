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

describe("When student taps see plan", () => {
  beforeEach(() => {
    jest.mocked(getStudentAppShop).mockResolvedValue(
      buildStudentAppShop({
        plans: [buildStudentAppShopPlan()],
        planStudents: [buildStudentAppPlanStudent()],
      }),
    );
  });

  it("Then the plan page opens", async () => {
    await renderStudentAppScreen(<StudentAppShopScreen />);

    await fireEvent.press(
      await screen.findByRole("button", {
        name: `${translate("student.shop.seePlan")} Principiante`,
      }),
    );

    expect(screen.getByText(translate("student.shop.planFor"))).toBeOnTheScreen();
  });
});
