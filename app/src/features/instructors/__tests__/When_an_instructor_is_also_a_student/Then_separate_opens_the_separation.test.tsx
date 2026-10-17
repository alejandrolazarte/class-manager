import { fireEvent, screen } from "@testing-library/react-native";
import { listInstructorsIncludingInactive } from "@/features/instructors/instructorsApi";
import { InstructorDetailScreen } from "@/features/instructors/screens/InstructorDetailScreen";
import { getTeam } from "@/features/members/membersApi";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { buildInstructor } from "@/testing/classGroupFactory";
import { routerMock } from "@/testing/expoRouterMock";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/instructors/instructorsApi");
jest.mock("@/features/members/membersApi");

const instructor = {
  ...buildInstructor({ fullName: "Ana Gómez" }),
  samePersonClientId: "client-1",
};

describe("When an instructor is also a student", () => {
  beforeEach(() => {
    jest.mocked(listInstructorsIncludingInactive).mockResolvedValue([instructor]);
    jest.mocked(getTeam).mockResolvedValue({ members: [], invitations: [] });
  });

  it("Then separate opens the separation", async () => {
    await renderWithProviders(<InstructorDetailScreen instructorId={instructor.id} />);
    expect(
      await screen.findByText(translate("people.alsoInStudents", { name: "Ana Gómez" })),
    ).toBeOnTheScreen();

    await fireEvent.press(screen.getByRole("button", { name: translate("people.separate.open") }));

    expect(routerMock.push).toHaveBeenCalledWith(routes.separateInstructor(instructor.id));
  });
});
