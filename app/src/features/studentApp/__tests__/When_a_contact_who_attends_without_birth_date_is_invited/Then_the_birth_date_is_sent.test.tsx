import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { InviteStudentAppSection } from "@/features/studentApp/components/InviteStudentAppSection";
import { inviteStudentApp } from "@/features/studentApp/studentAppApi";
import { translate } from "@/i18n/translate";
import { buildClient } from "@/testing/clientFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildStudent } from "@/testing/studentFactory";

jest.mock("@/features/studentApp/studentAppApi");

const contactAsStudent = buildStudent({ fullName: "Juan Chico", birthDate: null });
const client = buildClient({
  fullName: "Juan Chico",
  email: "juan@example.com",
  students: [contactAsStudent],
});

describe("When a contact who attends without birth date is invited", () => {
  beforeEach(() => {
    jest.mocked(inviteStudentApp).mockResolvedValue({
      id: "invitation-1",
      email: "juan@example.com",
      expiresAt: "2026-10-06T12:00:00Z",
    });
  });

  it("Then the birth date is sent", async () => {
    await renderWithProviders(<InviteStudentAppSection client={client} />);
    await fireEvent.press(
      await screen.findByRole("button", { name: translate("student.app.inviteAccessibility") }),
    );
    await fireEvent.changeText(
      screen.getByLabelText(translate("student.invite.birthDate")),
      "20061985",
    );

    await fireEvent.press(screen.getByRole("button", { name: translate("student.invite.send") }));

    await waitFor(() =>
      expect(inviteStudentApp).toHaveBeenCalledWith(client.id, {
        email: "juan@example.com",
        birthDate: "1985-06-20",
      }),
    );
  });
});
