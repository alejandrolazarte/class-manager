import { screen } from "@testing-library/react-native";
import { listInstructorsIncludingInactive } from "@/features/instructors/instructorsApi";
import { getTeam } from "@/features/members/membersApi";
import { TeamScreen } from "@/features/members/screens/TeamScreen";
import { translate } from "@/i18n/translate";
import { buildInstructor } from "@/testing/classGroupFactory";
import { buildMember } from "@/testing/memberFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/members/membersApi");
jest.mock("@/features/instructors/instructorsApi");

const instructorWithAccount = buildInstructor({ id: "with-account", fullName: "Marcos Díaz" });
const instructorWithoutAccount = buildInstructor({ id: "without-account", fullName: "Ana Ruiz" });

describe("When an instructor has an account", () => {
  beforeEach(() => {
    jest.mocked(getTeam).mockResolvedValue({
      members: [buildMember({ fullName: "Marcos Díaz", instructorId: instructorWithAccount.id })],
      invitations: [],
    });
    jest
      .mocked(listInstructorsIncludingInactive)
      .mockResolvedValue([instructorWithAccount, instructorWithoutAccount]);
  });

  it("Then it is only listed as a member", async () => {
    await renderWithProviders(<TeamScreen />);

    expect(
      await screen.findByRole("button", { name: instructorWithoutAccount.fullName }),
    ).toBeOnTheScreen();
    expect(screen.getByText(translate("team.instructorsWithoutAccount"))).toBeOnTheScreen();
    expect(screen.getAllByText(instructorWithAccount.fullName)).toHaveLength(1);
  });
});
