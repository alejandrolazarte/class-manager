import { fireEvent, screen } from "@testing-library/react-native";
import { InviteStudentAppSection } from "@/features/studentApp/components/InviteStudentAppSection";
import { inviteStudentApp } from "@/features/studentApp/studentAppApi";
import { translate } from "@/i18n/translate";
import { buildClient } from "@/testing/clientFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildStudent } from "@/testing/studentFactory";

jest.mock("@/features/studentApp/studentAppApi");

const child = buildStudent({ fullName: "Tomás Pérez" });
const client = buildClient({ email: "ana@example.com", students: [child] });

describe("When a child is invited with the client email", () => {
  it("Then no invitation is sent", async () => {
    await renderWithProviders(<InviteStudentAppSection client={client} student={child} />);
    await fireEvent.press(
      await screen.findByRole("button", { name: translate("student.app.inviteAccessibility") }),
    );
    await fireEvent.changeText(
      screen.getByLabelText(translate("student.invite.studentEmail", { name: child.fullName })),
      "Ana@example.com",
    );

    await fireEvent.press(screen.getByRole("button", { name: translate("student.invite.send") }));

    expect(
      await screen.findByText(translate("students.validation.emailOfAnotherPerson")),
    ).toBeOnTheScreen();
    expect(inviteStudentApp).not.toHaveBeenCalled();
  });
});
