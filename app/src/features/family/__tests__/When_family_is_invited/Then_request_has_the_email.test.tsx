import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { InviteFamilySection } from "@/features/family/components/InviteFamilySection";
import { inviteFamily } from "@/features/family/familyApi";
import { translate } from "@/i18n/translate";
import { buildClient } from "@/testing/clientFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/family/familyApi");

const client = buildClient({ email: "ana@example.com" });

describe("When family is invited", () => {
  beforeEach(() => {
    jest.mocked(inviteFamily).mockResolvedValue({
      id: "invitation-1",
      email: "ana.nueva@example.com",
      expiresAt: "2026-10-06T12:00:00Z",
    });
  });

  it("Then request has the email", async () => {
    await renderWithProviders(<InviteFamilySection client={client} />);
    await fireEvent.press(
      await screen.findByRole("button", { name: translate("family.invite.open") }),
    );
    await fireEvent.changeText(
      screen.getByLabelText(translate("family.invite.email")),
      "ana.nueva@example.com",
    );

    await fireEvent.press(screen.getByRole("button", { name: translate("family.invite.send") }));

    await waitFor(() =>
      expect(inviteFamily).toHaveBeenCalledWith(client.id, { email: "ana.nueva@example.com" }),
    );
    expect(await screen.findByText(translate("family.invite.sent"))).toBeOnTheScreen();
  });
});
