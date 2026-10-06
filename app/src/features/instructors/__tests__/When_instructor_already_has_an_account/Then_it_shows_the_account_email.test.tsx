import { screen } from "@testing-library/react-native";
import { listInstructorsIncludingInactive } from "@/features/instructors/instructorsApi";
import { InstructorDetailScreen } from "@/features/instructors/screens/InstructorDetailScreen";
import { getTeam } from "@/features/members/membersApi";
import { translate } from "@/i18n/translate";
import { buildInstructor } from "@/testing/classGroupFactory";
import { buildMember } from "@/testing/memberFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/instructors/instructorsApi");
jest.mock("@/features/members/membersApi");

const instructor = buildInstructor({ email: "laura.contacto@example.com" });
const member = buildMember({ email: "laura@example.com", instructorId: instructor.id });

describe("When instructor already has an account", () => {
  beforeEach(() => {
    jest.mocked(listInstructorsIncludingInactive).mockResolvedValue([instructor]);
    jest.mocked(getTeam).mockResolvedValue({ members: [member], invitations: [] });
  });

  it("Then it shows the account email", async () => {
    await renderWithProviders(<InstructorDetailScreen instructorId={instructor.id} />);

    expect(
      await screen.findByText(translate("instructors.app.Active", { email: member.email })),
    ).toBeOnTheScreen();
    expect(
      screen.queryByRole("button", { name: translate("instructors.app.inviteAccessibility") }),
    ).toBeNull();
  });
});
