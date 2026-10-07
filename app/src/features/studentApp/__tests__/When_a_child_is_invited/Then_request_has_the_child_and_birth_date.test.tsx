import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { InviteStudentAppSection } from "@/features/studentApp/components/InviteStudentAppSection";
import { inviteStudentApp } from "@/features/studentApp/studentAppApi";
import { translate } from "@/i18n/translate";
import { buildClient } from "@/testing/clientFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildStudent } from "@/testing/studentFactory";

jest.mock("@/features/studentApp/studentAppApi");

const child = buildStudent({ fullName: "Tomás Pérez", birthDate: null });
const client = buildClient({ email: "ana@example.com", students: [child] });

describe("When a child is invited", () => {
  beforeEach(() => {
    jest.mocked(inviteStudentApp).mockResolvedValue({
      id: "invitation-1",
      email: "tomas@example.com",
      expiresAt: "2026-10-06T12:00:00Z",
    });
  });

  it("Then request has the child and birth date", async () => {
    await renderWithProviders(<InviteStudentAppSection client={client} student={child} />);
    await fireEvent.press(
      await screen.findByRole("button", { name: translate("student.app.inviteAccessibility") }),
    );
    expect(
      screen.getByLabelText(translate("student.invite.studentEmail", { name: child.fullName })),
    ).toHaveDisplayValue("");
    await fireEvent.changeText(
      screen.getByLabelText(translate("student.invite.studentEmail", { name: child.fullName })),
      "tomas@example.com",
    );
    await fireEvent.changeText(
      screen.getByLabelText(translate("student.invite.birthDate")),
      "01052012",
    );

    await fireEvent.press(screen.getByRole("button", { name: translate("student.invite.send") }));

    await waitFor(() =>
      expect(inviteStudentApp).toHaveBeenCalledWith(client.id, {
        email: "tomas@example.com",
        studentId: child.id,
        birthDate: "2012-05-01",
      }),
    );
  });
});
