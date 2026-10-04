import { screen } from "@testing-library/react-native";
import { InviteStudentAppSection } from "@/features/studentApp/components/InviteStudentAppSection";
import { translate } from "@/i18n/translate";
import { buildClient } from "@/testing/clientFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/studentApp/studentAppApi");

const client = buildClient({
  email: "ana@example.com",
  appAccess: { status: "Invited", invitedEmail: "ana@example.com" },
});

describe("When student invitation is pending", () => {
  it("Then the email and resend are shown", async () => {
    await renderWithProviders(<InviteStudentAppSection client={client} />);

    expect(
      screen.getByText(translate("student.app.Invited", { email: "ana@example.com" })),
    ).toBeOnTheScreen();
    expect(
      screen.getByRole("button", { name: translate("student.app.resendAccessibility") }),
    ).toBeOnTheScreen();
  });
});
