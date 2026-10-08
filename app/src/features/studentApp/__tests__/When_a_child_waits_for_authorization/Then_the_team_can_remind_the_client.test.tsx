import { screen } from "@testing-library/react-native";
import { InviteStudentAppSection } from "@/features/studentApp/components/InviteStudentAppSection";
import { translate } from "@/i18n/translate";
import { buildClient } from "@/testing/clientFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildStudent } from "@/testing/studentFactory";

jest.mock("@/features/studentApp/studentAppApi");

const child = buildStudent({
  fullName: "Tomás Pérez",
  email: "tomas@example.com",
  appAccess: { status: "AwaitingGuardianConsent", invitedEmail: "ana@example.com" },
});
const client = buildClient({ email: "ana@example.com", students: [child] });

describe("When a child waits for authorization", () => {
  it("Then the team can remind the client", async () => {
    await renderWithProviders(<InviteStudentAppSection client={client} student={child} />);

    expect(
      await screen.findByRole("button", {
        name: translate("student.app.remindGuardianAccessibility"),
      }),
    ).toBeOnTheScreen();
  });
});
