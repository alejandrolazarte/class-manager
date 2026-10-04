import { screen } from "@testing-library/react-native";
import { getStudentAppHome, getStudentAppShop } from "@/features/studentApp/studentAppApi";
import { StudentAppHomeScreen } from "@/features/studentApp/screens/StudentAppHomeScreen";
import { translateCount } from "@/i18n/translate";
import { buildStudentAppShop, buildStudentAppHome } from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");

describe("When student pays per class", () => {
  beforeEach(() => {
    jest.mocked(getStudentAppShop).mockResolvedValue(buildStudentAppShop());
    jest.mocked(getStudentAppHome).mockResolvedValue(
      buildStudentAppHome({
        billing: {
          kind: "ClassPacks",
          monthlyFee: null,
          classes: { availableClasses: 5, unpaidClasses: 0 },
        },
      }),
    );
  });

  it("Then classes left are shown", async () => {
    await renderStudentAppScreen(<StudentAppHomeScreen />);

    expect(
      await screen.findByText(translateCount("student.classes.available", 5)),
    ).toBeOnTheScreen();
  });
});
