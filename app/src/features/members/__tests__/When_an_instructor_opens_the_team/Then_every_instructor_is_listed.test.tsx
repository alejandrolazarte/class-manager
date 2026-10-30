import { screen } from "@testing-library/react-native";
import { listInstructorsIncludingInactive } from "@/features/instructors/instructorsApi";
import { getTeam } from "@/features/members/membersApi";
import { TeamScreen } from "@/features/members/screens/TeamScreen";
import { translate } from "@/i18n/translate";
import { buildInstructor } from "@/testing/classGroupFactory";
import { buildInstructorMember } from "@/testing/memberFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/members/membersApi");
jest.mock("@/features/instructors/instructorsApi");

const ownInstructor = buildInstructor({ id: "instructor-of-member", fullName: "Marcos Díaz" });
const otherInstructor = buildInstructor({ id: "other", fullName: "Ana Ruiz" });

describe("When an instructor opens the team", () => {
  beforeEach(() => {
    jest
      .mocked(listInstructorsIncludingInactive)
      .mockResolvedValue([ownInstructor, otherInstructor]);
  });

  it("Then every instructor is listed", async () => {
    await renderWithProviders(<TeamScreen />, { member: buildInstructorMember(ownInstructor.id) });

    expect(await screen.findByText(ownInstructor.fullName)).toBeOnTheScreen();
    expect(screen.getByText(otherInstructor.fullName)).toBeOnTheScreen();
    expect(screen.queryByText(translate("team.members"))).toBeNull();
    expect(screen.queryByRole("button", { name: translate("team.add") })).toBeNull();
    expect(getTeam).not.toHaveBeenCalled();
  });
});
