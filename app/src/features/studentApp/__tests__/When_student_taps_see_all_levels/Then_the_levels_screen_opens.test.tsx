import { fireEvent, screen } from "@testing-library/react-native";
import { getStudentAppHome } from "@/features/studentApp/studentAppApi";
import { StudentAppProgressScreen } from "@/features/studentApp/screens/StudentAppProgressScreen";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { routerMock } from "@/testing/expoRouterMock";
import { buildStudentAppHome } from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");
jest.mock("@/features/studentApp/seenMedalsStorage");

describe("When student taps see all levels", () => {
  beforeEach(() => {
    jest.mocked(getStudentAppHome).mockResolvedValue(buildStudentAppHome());
  });

  it("Then the levels screen opens", async () => {
    await renderStudentAppScreen(<StudentAppProgressScreen />);

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("student.progress.allLevels") }),
    );

    expect(routerMock.push).toHaveBeenCalledWith(routes.studentAppLevels);
  });
});
