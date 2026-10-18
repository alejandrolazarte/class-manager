import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { InviteStudentAppSection } from "@/features/studentApp/components/InviteStudentAppSection";
import { inviteStudentApp } from "@/features/studentApp/studentAppApi";
import { translate } from "@/i18n/translate";
import { buildClient } from "@/testing/clientFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/studentApp/studentAppApi");

const client = buildClient({
  email: "ana.nueva@example.com",
  appAccess: { status: "Invited", invitedEmail: "ana@example.com" },
});

describe("When the contact email changed after the invitation", () => {
  beforeEach(() => {
    jest.mocked(inviteStudentApp).mockResolvedValue({
      id: "invitation-2",
      email: "ana.nueva@example.com",
      expiresAt: "2026-10-06T12:00:00Z",
    });
  });

  it("Then the invitation is resent to the new email", async () => {
    await renderWithProviders(<InviteStudentAppSection client={client} />);

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("student.app.resendAccessibility") }),
    );

    await waitFor(() =>
      expect(inviteStudentApp).toHaveBeenCalledWith(client.id, { email: "ana.nueva@example.com" }),
    );
  });
});
