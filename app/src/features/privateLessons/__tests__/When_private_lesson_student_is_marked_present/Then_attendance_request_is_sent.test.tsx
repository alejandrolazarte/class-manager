import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import {
  getPrivateLesson,
  recordPrivateLessonAttendance,
} from "@/features/privateLessons/privateLessonsApi";
import { PrivateLessonScreen } from "@/features/privateLessons/screens/PrivateLessonScreen";
import { translate } from "@/i18n/translate";
import { buildPrivateLesson, buildPrivateLessonStudent } from "@/testing/privateLessonFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/privateLessons/privateLessonsApi");

const lesson = buildPrivateLesson();
const student = buildPrivateLessonStudent();

describe("When private lesson student is marked present", () => {
  beforeEach(() => {
    jest.mocked(getPrivateLesson).mockResolvedValue(lesson);
    jest.mocked(recordPrivateLessonAttendance).mockResolvedValue(undefined);
  });

  it("Then attendance request is sent", async () => {
    await renderWithProviders(<PrivateLessonScreen privateLessonId={lesson.id} />);

    await fireEvent.press(
      await screen.findByRole("button", {
        name: translate("sessions.attendance.presentFor", { name: student.studentFullName }),
      }),
    );

    await waitFor(() =>
      expect(recordPrivateLessonAttendance).toHaveBeenCalledWith(
        lesson.id,
        student.studentId,
        "Present",
      ),
    );
  });
});
