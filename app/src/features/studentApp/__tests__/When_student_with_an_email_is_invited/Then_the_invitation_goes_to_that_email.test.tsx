import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { InviteStudentAppSection } from "@/features/studentApp/components/InviteStudentAppSection";
import { inviteStudentApp } from "@/features/studentApp/studentAppApi";
import { translate } from "@/i18n/translate";
import { buildClient } from "@/testing/clientFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/studentApp/studentAppApi");

const client = buildClient({ email: "ana@example.com" });

describe("When student with an email is invited", () => {
  beforeEach(() => {
    jest.mocked(inviteStudentApp).mockResolvedValue({
      id: "invitation-1",
      email: "ana@example.com",
      expiresAt: "2026-10-06T12:00:00Z",
    });
  });

  it("Then the invitation goes to that email", async () => {
    await renderWithProviders(<InviteStudentAppSection client={client} />);

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("student.app.inviteAccessibility") }),
    );
    expect(screen.getByLabelText(translate("student.invite.email"))).toHaveDisplayValue(
      "ana@example.com",
    );

    await fireEvent.press(screen.getByRole("button", { name: translate("student.invite.send") }));

    await waitFor(() =>
      expect(inviteStudentApp).toHaveBeenCalledWith(client.id, { email: "ana@example.com" }),
    );
  });
});
