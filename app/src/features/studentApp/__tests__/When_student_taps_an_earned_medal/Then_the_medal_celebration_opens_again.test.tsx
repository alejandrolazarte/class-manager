import { fireEvent, screen } from "@testing-library/react-native";
import { seenMedalsStorage } from "@/features/studentApp/seenMedalsStorage";
import { getStudentAppHome } from "@/features/studentApp/studentAppApi";
import { StudentAppProgressScreen } from "@/features/studentApp/screens/StudentAppProgressScreen";
import { translate } from "@/i18n/translate";
import {
  buildStudentAppAttendance,
  buildStudentAppHome,
  buildAccountStudent,
} from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");
jest.mock("@/features/studentApp/seenMedalsStorage");

describe("When student taps an earned medal", () => {
  beforeEach(() => {
    jest.mocked(seenMedalsStorage.read).mockResolvedValue(["FirstClass", "TenClasses"]);
    jest.mocked(getStudentAppHome).mockResolvedValue(
      buildStudentAppHome({
        students: [
          buildAccountStudent({
            attendance: buildStudentAppAttendance({
              attendedClasses: 10,
              medals: ["FirstClass", "TenClasses"],
            }),
          }),
        ],
      }),
    );
  });

  it("Then the medal celebration opens again", async () => {
    await renderStudentAppScreen(<StudentAppProgressScreen />);

    await fireEvent.press(
      await screen.findByRole("button", {
        name: translate("student.progress.medalEarned", {
          name: translate("student.medals.TenClasses"),
        }),
      }),
    );

    expect(await screen.findByText(translate("student.celebration.title"))).toBeOnTheScreen();
    expect(screen.getByText(translate("student.medals.description.TenClasses"))).toBeOnTheScreen();
  });
});
