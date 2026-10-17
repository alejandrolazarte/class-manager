import { fireEvent, screen } from "@testing-library/react-native";
import { ApiError } from "@/api/httpClient";
import { InviteStudentAppSection } from "@/features/studentApp/components/InviteStudentAppSection";
import { inviteStudentApp } from "@/features/studentApp/studentAppApi";
import { userErrorCodes } from "@/features/users/userErrorCodes";
import { translate } from "@/i18n/translate";
import { buildClient } from "@/testing/clientFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildStudent } from "@/testing/studentFactory";

jest.mock("@/features/studentApp/studentAppApi");

const badRequestStatus = 400;
const child = buildStudent({ fullName: "Tomás Pérez" });
const client = buildClient({ email: "ana@example.com", students: [child] });

describe("When a child is invited with the email of another person", () => {
  beforeEach(() => {
    jest
      .mocked(inviteStudentApp)
      .mockRejectedValue(new ApiError(badRequestStatus, { code: userErrorCodes.emailTaken }));
  });

  it("Then the email is asked again", async () => {
    await renderWithProviders(<InviteStudentAppSection client={client} student={child} />);
    await fireEvent.press(
      await screen.findByRole("button", { name: translate("student.app.inviteAccessibility") }),
    );
    await fireEvent.changeText(
      screen.getByLabelText(translate("student.invite.studentEmail", { name: child.fullName })),
      "sofia@example.com",
    );

    await fireEvent.press(screen.getByRole("button", { name: translate("student.invite.send") }));

    expect(await screen.findByText(translate("apiErrors.user.email_taken"))).toBeOnTheScreen();
  });
});
