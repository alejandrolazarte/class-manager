import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { ApiError } from "@/api/httpClient";
import { InviteStudentAppSection } from "@/features/studentApp/components/InviteStudentAppSection";
import { inviteStudentApp } from "@/features/studentApp/studentAppApi";
import { userErrorCodes } from "@/features/users/userErrorCodes";
import { translate } from "@/i18n/translate";
import { buildClient } from "@/testing/clientFactory";
import { confirmContactIsAdult } from "@/testing/confirmContactIsAdult";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/studentApp/studentAppApi");

const conflictStatus = 409;
const client = buildClient({ email: "ana@example.com" });
const instructorUserId = "user-of-the-instructor";

describe("When the app invitation email belongs to an instructor", () => {
  beforeEach(() => {
    jest
      .mocked(inviteStudentApp)
      .mockRejectedValueOnce(
        new ApiError(conflictStatus, {
          code: userErrorCodes.samePerson,
          samePersonUserId: instructorUserId,
          samePersonFullName: "Ana Gómez",
        }),
      )
      .mockResolvedValueOnce({
        id: "invitation-1",
        email: "ana@example.com",
        expiresAt: "2026-10-24T12:00:00Z",
      });
  });

  it("Then it is sent after confirming the same person", async () => {
    await renderWithProviders(<InviteStudentAppSection client={client} />);
    await fireEvent.press(
      await screen.findByRole("button", { name: translate("student.app.inviteAccessibility") }),
    );
    await confirmContactIsAdult(client.fullName);
    await fireEvent.press(screen.getByRole("button", { name: translate("student.invite.send") }));
    expect(
      await screen.findByText(translate("people.samePerson.question", { name: "Ana Gómez" })),
    ).toBeOnTheScreen();
    expect(inviteStudentApp).toHaveBeenCalledTimes(1);

    await fireEvent.press(
      screen.getByRole("button", { name: translate("people.samePerson.confirm") }),
    );

    await waitFor(() =>
      expect(inviteStudentApp).toHaveBeenLastCalledWith(client.id, {
        email: "ana@example.com",
        confirmsContactIsAdult: true,
        samePersonUserId: instructorUserId,
      }),
    );
  });
});
