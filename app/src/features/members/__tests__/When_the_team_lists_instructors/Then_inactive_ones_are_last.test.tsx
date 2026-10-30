import { screen } from "@testing-library/react-native";
import { listInstructorsIncludingInactive } from "@/features/instructors/instructorsApi";
import { getTeam } from "@/features/members/membersApi";
import { TeamScreen } from "@/features/members/screens/TeamScreen";
import { buildInstructor } from "@/testing/classGroupFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/members/membersApi");
jest.mock("@/features/instructors/instructorsApi");

describe("When the team lists instructors", () => {
  beforeEach(() => {
    jest.mocked(getTeam).mockResolvedValue({ members: [], invitations: [] });
    jest
      .mocked(listInstructorsIncludingInactive)
      .mockResolvedValue([
        buildInstructor({ id: "inactive", fullName: "Ana Ruiz", isActive: false }),
        buildInstructor({ id: "active", fullName: "Laura Gómez" }),
      ]);
  });

  it("Then inactive ones are last", async () => {
    await renderWithProviders(<TeamScreen />);

    const instructorButtons = await screen.findAllByRole("button", { name: /Gómez|Ruiz/ });
    expect(instructorButtons.map((button) => button.props.accessibilityLabel)).toEqual([
      "Laura Gómez",
      "Ana Ruiz",
    ]);
  });
});
