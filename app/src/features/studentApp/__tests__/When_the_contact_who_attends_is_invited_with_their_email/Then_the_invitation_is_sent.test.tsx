import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { InviteStudentAppSection } from "@/features/studentApp/components/InviteStudentAppSection";
import { inviteStudentApp } from "@/features/studentApp/studentAppApi";
import { translate } from "@/i18n/translate";
import { buildClient } from "@/testing/clientFactory";
import { confirmContactIsAdult } from "@/testing/confirmContactIsAdult";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildStudent } from "@/testing/studentFactory";

jest.mock("@/features/studentApp/studentAppApi");

const client = buildClient({
  email: "ana@example.com",
  students: [
    buildStudent({ fullName: "Ana Pérez", birthDate: "1990-05-02", email: "ana@example.com" }),
  ],
});

describe("When the contact who attends is invited with their email", () => {
  beforeEach(() => {
    jest.mocked(inviteStudentApp).mockResolvedValue({
      id: "invitation-1",
      email: "ana@example.com",
      expiresAt: "2026-10-06T12:00:00Z",
    });
  });

  it("Then the invitation is sent", async () => {
    await renderWithProviders(<InviteStudentAppSection client={client} />);
    await fireEvent.press(
      await screen.findByRole("button", { name: translate("student.app.inviteAccessibility") }),
    );

    await confirmContactIsAdult(client.fullName);
    await fireEvent.press(screen.getByRole("button", { name: translate("student.invite.send") }));

    await waitFor(() =>
      expect(inviteStudentApp).toHaveBeenCalledWith(client.id, {
        email: "ana@example.com",
        confirmsContactIsAdult: true,
      }),
    );
  });
});
