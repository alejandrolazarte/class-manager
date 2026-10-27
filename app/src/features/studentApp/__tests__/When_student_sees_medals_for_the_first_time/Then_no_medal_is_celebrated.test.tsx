import { screen, waitFor } from "@testing-library/react-native";
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

const studentId = "0192f0c5-0000-7000-8000-000000000009";

describe("When student sees medals for the first time", () => {
  beforeEach(() => {
    jest.mocked(seenMedalsStorage.read).mockResolvedValue(null);
    jest.mocked(getStudentAppHome).mockResolvedValue(
      buildStudentAppHome({
        students: [
          buildAccountStudent({
            id: studentId,
            attendance: buildStudentAppAttendance({
              attendedClasses: 10,
              medals: ["FirstClass", "TenClasses"],
            }),
          }),
        ],
      }),
    );
  });

  it("Then no medal is celebrated", async () => {
    await renderStudentAppScreen(<StudentAppProgressScreen />);

    await waitFor(() =>
      expect(seenMedalsStorage.write).toHaveBeenCalledWith(studentId, ["FirstClass", "TenClasses"]),
    );
    expect(screen.queryByText(translate("student.celebration.title"))).toBeNull();
  });
});
