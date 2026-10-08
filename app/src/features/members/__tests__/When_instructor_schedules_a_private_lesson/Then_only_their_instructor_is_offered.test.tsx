import { screen } from "@testing-library/react-native";
import { listActiveInstructors } from "@/features/instructors/instructorsApi";
import { PrivateLessonFormScreen } from "@/features/privateLessons/screens/PrivateLessonFormScreen";
import { translate } from "@/i18n/translate";
import { buildInstructor } from "@/testing/classGroupFactory";
import { buildInstructorMember } from "@/testing/memberFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { withRequiredMark } from "@/ui/RequiredFieldsLegend";

jest.mock("@/features/instructors/instructorsApi");
jest.mock("@/features/privateLessons/privateLessonsApi");
jest.mock("@/features/students/studentsApi");

const instructorOfMember = buildInstructor({ id: "instructor-of-member", fullName: "Marcos Díaz" });
const otherInstructor = buildInstructor({ id: "instructor-other", fullName: "Laura Gómez" });

describe("When instructor schedules a private lesson", () => {
  beforeEach(() => {
    jest.mocked(listActiveInstructors).mockResolvedValue([instructorOfMember, otherInstructor]);
  });

  it("Then only their instructor is offered", async () => {
    await renderWithProviders(<PrivateLessonFormScreen initialDate="2026-10-06" />, {
      member: buildInstructorMember(instructorOfMember.id),
    });

    expect(
      await screen.findByText(withRequiredMark(translate("privateLessons.form.instructor"))),
    ).toBeOnTheScreen();
    expect(screen.getByText(instructorOfMember.fullName)).toBeOnTheScreen();
    expect(screen.queryByText(otherInstructor.fullName)).toBeNull();
  });
});
