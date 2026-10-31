import { fireEvent, screen } from "@testing-library/react-native";
import { getStudentAppHome, getStudentAppShop } from "@/features/studentApp/studentAppApi";
import { StudentAppHomeScreen } from "@/features/studentApp/screens/StudentAppHomeScreen";
import { translateCount } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { routerMock } from "@/testing/expoRouterMock";
import { buildStudentAppHome, buildStudentAppShop } from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");

describe("When student owes pack classes", () => {
  beforeEach(() => {
    jest.mocked(getStudentAppShop).mockResolvedValue(buildStudentAppShop());
    jest.mocked(getStudentAppHome).mockResolvedValue(
      buildStudentAppHome({
        billing: {
          monthlyFee: null,
          classes: { availableClasses: 8, unpaidClasses: 3 },
        },
      }),
    );
  });

  it("Then the billing tile opens the pack classes", async () => {
    await renderStudentAppScreen(<StudentAppHomeScreen />);

    await fireEvent.press(await screen.findByText(translateCount("student.classes.unpaid", 3)));

    expect(routerMock.navigate).toHaveBeenCalledWith(routes.studentAppClassPacks);
  });
});
