import { screen } from "@testing-library/react-native";
import { listInstructorsIncludingInactive } from "@/features/instructors/instructorsApi";
import { getTeam } from "@/features/members/membersApi";
import { TeamScreen } from "@/features/members/screens/TeamScreen";
import { translate } from "@/i18n/translate";
import { buildMember } from "@/testing/memberFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/members/membersApi");
jest.mock("@/features/instructors/instructorsApi");

const viewer = buildMember({
  id: "member-viewer",
  fullName: "Laura Gómez",
  role: "Viewer",
  instructorId: null,
});
const platformOperator = buildMember({
  id: "member-operator",
  fullName: "Soporte Laztty",
  email: "support@example.com",
  role: "PlatformOperator",
  instructorId: null,
});

describe("When the team has a platform operator", () => {
  beforeEach(() => {
    jest
      .mocked(getTeam)
      .mockResolvedValue({ members: [viewer, platformOperator], invitations: [] });
    jest.mocked(listInstructorsIncludingInactive).mockResolvedValue([]);
  });

  it("Then it is listed but not editable", async () => {
    await renderWithProviders(<TeamScreen />);

    expect(await screen.findByText(translate("roles.PlatformOperator"))).toBeOnTheScreen();
    expect(screen.getByRole("button", { name: viewer.fullName })).toBeOnTheScreen();
    expect(screen.queryByRole("button", { name: platformOperator.fullName })).toBeNull();
  });
});
