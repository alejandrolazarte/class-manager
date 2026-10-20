import { fireEvent, screen } from "@testing-library/react-native";
import { getStudentAppHome } from "@/features/studentApp/studentAppApi";
import { StudentAppClassesScreen } from "@/features/studentApp/screens/StudentAppClassesScreen";
import { todayIsoDate } from "@/dates/dates";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { routerMock } from "@/testing/expoRouterMock";
import {
  buildAccountStudent,
  buildStudentAppHome,
  buildStudentAppNextClass,
} from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");

const classGroupId = "0192f0c5-0000-7000-8000-000000000010";
const className = "Grupo inicial";
const today = todayIsoDate();

describe("When student taps a class", () => {
  beforeEach(() => {
    jest.mocked(getStudentAppHome).mockResolvedValue(
      buildStudentAppHome({
        students: [
          buildAccountStudent({
            nextClasses: [
              buildStudentAppNextClass({
                name: className,
                classGroupId,
                date: today,
                startTime: "18:00",
              }),
            ],
          }),
        ],
      }),
    );
  });

  it("Then the class detail opens", async () => {
    await renderStudentAppScreen(<StudentAppClassesScreen />);

    await fireEvent.press(
      await screen.findByRole("button", {
        name: translate("student.classDetail.open", { className }),
      }),
    );

    expect(routerMock.push).toHaveBeenCalledWith(
      routes.studentAppClass({ classGroupId, date: today, startTime: "18:00" }),
    );
  });
});
