import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { ApiError } from "@/api/httpClient";
import { createInstructor } from "@/features/instructors/instructorsApi";
import { AddTeamMemberScreen } from "@/features/members/screens/AddTeamMemberScreen";
import { userErrorCodes } from "@/features/users/userErrorCodes";
import { translate } from "@/i18n/translate";
import { buildInstructor } from "@/testing/classGroupFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/instructors/instructorsApi");

const conflictStatus = 409;
const studentUserId = "user-of-the-student";

describe("When an added instructor has the email of a student", () => {
  beforeEach(() => {
    jest
      .mocked(createInstructor)
      .mockRejectedValueOnce(
        new ApiError(conflictStatus, {
          code: userErrorCodes.samePerson,
          samePersonUserId: studentUserId,
          samePersonFullName: "Ana Gómez",
          samePersonField: "Email",
        }),
      )
      .mockResolvedValueOnce(buildInstructor());
  });

  it("Then it is saved after confirming the same person", async () => {
    await renderWithProviders(<AddTeamMemberScreen />);
    await fireEvent.changeText(
      screen.getByLabelText(translate("instructors.form.fullName")),
      "Ana Gómez",
    );
    await fireEvent.changeText(
      screen.getByLabelText(translate("team.invite.email")),
      "ana@example.com",
    );
    await fireEvent.press(screen.getByRole("button", { name: translate("common.save") }));
    expect(
      await screen.findByText(translate("people.samePerson.question", { name: "Ana Gómez" })),
    ).toBeOnTheScreen();
    expect(createInstructor).toHaveBeenCalledTimes(1);

    await fireEvent.press(
      screen.getByRole("button", { name: translate("people.samePerson.confirm") }),
    );

    await waitFor(() =>
      expect(createInstructor).toHaveBeenLastCalledWith({
        fullName: "Ana Gómez",
        email: "ana@example.com",
        samePersonUserId: studentUserId,
      }),
    );
  });
});
