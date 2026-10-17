import { fireEvent, screen } from "@testing-library/react-native";
import { InviteStudentAppSection } from "@/features/studentApp/components/InviteStudentAppSection";
import { inviteStudentApp } from "@/features/studentApp/studentAppApi";
import { translate } from "@/i18n/translate";
import { buildClient } from "@/testing/clientFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/studentApp/studentAppApi");

const client = buildClient({ fullName: "Ana Pérez", email: "ana@example.com" });

describe("When the contact is invited", () => {
  it("Then send waits for the adult confirmation", async () => {
    await renderWithProviders(<InviteStudentAppSection client={client} />);
    await fireEvent.press(
      await screen.findByRole("button", { name: translate("student.app.inviteAccessibility") }),
    );

    await fireEvent.press(screen.getByRole("button", { name: translate("student.invite.send") }));

    expect(screen.getByRole("button", { name: translate("student.invite.send") })).toBeDisabled();
    expect(inviteStudentApp).not.toHaveBeenCalled();
  });
});
