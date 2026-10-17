import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { InviteStudentAppSection } from "@/features/studentApp/components/InviteStudentAppSection";
import { inviteStudentApp } from "@/features/studentApp/studentAppApi";
import { translate } from "@/i18n/translate";
import { buildClient } from "@/testing/clientFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildSamePersonError } from "@/testing/samePersonError";
import { buildStudent } from "@/testing/studentFactory";

jest.mock("@/features/studentApp/studentAppApi");

const instructorUserId = "0192f0c4-0000-7000-8000-000000000010";
const child = buildStudent({ fullName: "Tomás Pérez" });
const client = buildClient({ email: "ana@example.com", students: [child] });

describe("When a child is invited with the email of an instructor", () => {
  beforeEach(() => {
    jest
      .mocked(inviteStudentApp)
      .mockRejectedValueOnce(buildSamePersonError(instructorUserId, "Tomás Pérez", ["Instructor"]))
      .mockResolvedValueOnce({
        id: "invitation-1",
        email: "tomas@example.com",
        expiresAt: "2026-10-10T12:00:00Z",
      });
  });

  it("Then the child joins the instructor once confirmed", async () => {
    await renderWithProviders(<InviteStudentAppSection client={client} student={child} />);
    await fireEvent.press(
      await screen.findByRole("button", { name: translate("student.app.inviteAccessibility") }),
    );
    await fireEvent.changeText(
      screen.getByLabelText(translate("student.invite.studentEmail", { name: child.fullName })),
      "tomas@example.com",
    );
    await fireEvent.press(screen.getByRole("button", { name: translate("student.invite.send") }));

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("people.samePerson.confirm") }),
    );

    await waitFor(() =>
      expect(inviteStudentApp).toHaveBeenLastCalledWith(
        client.id,
        expect.objectContaining({ email: "tomas@example.com", samePersonUserId: instructorUserId }),
      ),
    );
  });
});
