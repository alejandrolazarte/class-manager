import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { listInstructorsIncludingInactive } from "@/features/instructors/instructorsApi";
import { getTeam, resendInvitation } from "@/features/members/membersApi";
import { TeamScreen } from "@/features/members/screens/TeamScreen";
import { translate } from "@/i18n/translate";
import { buildInvitation } from "@/testing/memberFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/members/membersApi");
jest.mock("@/features/instructors/instructorsApi");

const invitation = buildInvitation();

describe("When invitation is resent", () => {
  beforeEach(() => {
    jest.mocked(getTeam).mockResolvedValue({ members: [], invitations: [invitation] });
    jest.mocked(listInstructorsIncludingInactive).mockResolvedValue([]);
    jest.mocked(resendInvitation).mockResolvedValue(invitation);
  });

  it("Then request has the invitation", async () => {
    await renderWithProviders(<TeamScreen />);

    await fireEvent.press(
      await screen.findByRole("button", {
        name: `${translate("team.resend")} ${invitation.email}`,
      }),
    );

    await waitFor(() => expect(resendInvitation).toHaveBeenCalledWith(invitation.id));
    expect(await screen.findByText(translate("team.resent"))).toBeOnTheScreen();
  });
});
