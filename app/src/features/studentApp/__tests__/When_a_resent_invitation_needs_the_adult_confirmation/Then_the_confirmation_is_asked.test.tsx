import { fireEvent, screen } from "@testing-library/react-native";
import { ApiError } from "@/api/httpClient";
import { InviteStudentAppSection } from "@/features/studentApp/components/InviteStudentAppSection";
import { inviteStudentApp } from "@/features/studentApp/studentAppApi";
import { translate } from "@/i18n/translate";
import { buildClient } from "@/testing/clientFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/studentApp/studentAppApi");

const badRequestStatus = 400;
const client = buildClient({
  fullName: "Ana Pérez",
  email: "ana@example.com",
  appAccess: { status: "Invited", invitedEmail: "ana@example.com" },
});

describe("When a resent invitation needs the adult confirmation", () => {
  beforeEach(() => {
    jest.mocked(inviteStudentApp).mockRejectedValue(
      new ApiError(badRequestStatus, {
        code: "student_invitation.contact_adult_confirmation_required",
      }),
    );
  });

  it("Then the confirmation is asked", async () => {
    await renderWithProviders(<InviteStudentAppSection client={client} />);

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("student.app.resendAccessibility") }),
    );

    expect(
      await screen.findByRole("checkbox", {
        name: translate("student.invite.confirmsAdult", { name: client.fullName }),
      }),
    ).toBeOnTheScreen();
  });
});
