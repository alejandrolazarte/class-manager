import { fireEvent, screen } from "@testing-library/react-native";
import { listInstructorsIncludingInactive } from "@/features/instructors/instructorsApi";
import { getTeam } from "@/features/members/membersApi";
import { TeamScreen } from "@/features/members/screens/TeamScreen";
import { routes } from "@/navigation/routes";
import { buildInstructor } from "@/testing/classGroupFactory";
import { routerMock } from "@/testing/expoRouterMock";
import { buildMember } from "@/testing/memberFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/members/membersApi");
jest.mock("@/features/instructors/instructorsApi");

const instructorOfMember = buildInstructor({ id: "instructor-of-member" });
const member = buildMember({ instructorId: instructorOfMember.id });

describe("When a member is an instructor", () => {
  beforeEach(() => {
    jest.mocked(listInstructorsIncludingInactive).mockResolvedValue([instructorOfMember]);
    jest.mocked(getTeam).mockResolvedValue({ members: [member], invitations: [] });
  });

  it("Then the team opens the instructor profile", async () => {
    await renderWithProviders(<TeamScreen />);

    await fireEvent.press(await screen.findByRole("button", { name: member.fullName }));

    expect(routerMock.push).toHaveBeenCalledWith(routes.instructor(instructorOfMember.id));
  });
});
