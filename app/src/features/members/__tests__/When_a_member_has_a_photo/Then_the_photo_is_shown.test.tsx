import { screen } from "@testing-library/react-native";
import { listInstructorsIncludingInactive } from "@/features/instructors/instructorsApi";
import { getTeam } from "@/features/members/membersApi";
import { TeamScreen } from "@/features/members/screens/TeamScreen";
import { buildMember } from "@/testing/memberFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/members/membersApi");
jest.mock("@/features/instructors/instructorsApi");

const photoUrl = "https://files.example.com/private/marcos.webp?sig=abc";

describe("When a member has a photo", () => {
  beforeEach(() => {
    jest
      .mocked(getTeam)
      .mockResolvedValue({ members: [buildMember({ photoUrl })], invitations: [] });
    jest.mocked(listInstructorsIncludingInactive).mockResolvedValue([]);
  });

  it("Then the photo is shown", async () => {
    await renderWithProviders(<TeamScreen />);

    expect(await screen.findByTestId("avatar-photo", { includeHiddenElements: true })).toHaveProp(
      "source",
      { uri: photoUrl },
    );
  });
});
