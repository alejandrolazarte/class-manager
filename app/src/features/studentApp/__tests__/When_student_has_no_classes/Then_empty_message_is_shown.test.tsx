import { screen } from "@testing-library/react-native";
import { getStudentAppHome, getStudentAppShop } from "@/features/studentApp/studentAppApi";
import { StudentAppHomeScreen } from "@/features/studentApp/screens/StudentAppHomeScreen";
import { translate } from "@/i18n/translate";
import {
  buildStudentAppShop,
  buildStudentAppHome,
  buildAccountStudent,
} from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");

describe("When student has no classes", () => {
  beforeEach(() => {
    jest.mocked(getStudentAppShop).mockResolvedValue(buildStudentAppShop());
    jest
      .mocked(getStudentAppHome)
      .mockResolvedValue(
        buildStudentAppHome({ students: [buildAccountStudent({ nextClasses: [] })] }),
      );
  });

  it("Then empty message is shown", async () => {
    await renderStudentAppScreen(<StudentAppHomeScreen />);

    expect(await screen.findByText(translate("student.student.noClasses"))).toBeOnTheScreen();
  });
});
