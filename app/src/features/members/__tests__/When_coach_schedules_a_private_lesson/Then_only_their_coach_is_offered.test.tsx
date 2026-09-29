import { screen } from "@testing-library/react-native";
import { listActiveInstructors } from "@/features/instructors/instructorsApi";
import { PrivateLessonFormScreen } from "@/features/privateLessons/screens/PrivateLessonFormScreen";
import { translate } from "@/i18n/translate";
import { buildInstructor } from "@/testing/classGroupFactory";
import { buildCoach } from "@/testing/memberFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/instructors/instructorsApi");
jest.mock("@/features/privateLessons/privateLessonsApi");
jest.mock("@/features/students/studentsApi");

const coachInstructor = buildInstructor({ id: "instructor-coach", fullName: "Marcos Díaz" });
const otherInstructor = buildInstructor({ id: "instructor-other", fullName: "Laura Gómez" });

describe("When coach schedules a private lesson", () => {
  beforeEach(() => {
    jest.mocked(listActiveInstructors).mockResolvedValue([coachInstructor, otherInstructor]);
  });

  it("Then only their coach is offered", async () => {
    await renderWithProviders(<PrivateLessonFormScreen initialDate="2026-10-06" />, {
      member: buildCoach(coachInstructor.id),
    });

    expect(await screen.findByText(translate("privateLessons.form.coach"))).toBeOnTheScreen();
    expect(screen.getByText(coachInstructor.fullName)).toBeOnTheScreen();
    expect(screen.queryByText(otherInstructor.fullName)).toBeNull();
  });
});
