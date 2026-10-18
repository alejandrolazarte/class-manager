import { fireEvent, screen } from "@testing-library/react-native";
import { ApiError } from "@/api/httpClient";
import { registerClient } from "@/features/clients/clientsApi";
import { RegisterClientScreen } from "@/features/clients/screens/RegisterClientScreen";
import { userErrorCodes } from "@/features/users/userErrorCodes";
import { translate } from "@/i18n/translate";
import {
  fillRegisterClientEmail,
  fillRegisterClientForm,
  setSendAppInvitation,
  submitRegisterClientForm,
} from "@/testing/fillRegisterClientForm";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/clients/clientsApi");

const conflictStatus = 409;

describe("When the registration same person question is answered with back", () => {
  beforeEach(() => {
    jest.mocked(registerClient).mockRejectedValue(
      new ApiError(conflictStatus, {
        code: userErrorCodes.samePerson,
        samePersonUserId: "user-of-the-instructor",
        samePersonFullName: "Ana Gómez",
        samePersonField: "Email",
      }),
    );
  });

  it("Then the email error is shown", async () => {
    await renderWithProviders(<RegisterClientScreen />);
    await fillRegisterClientForm("Martina Gómez", "11 2233-4455");
    await fillRegisterClientEmail("ana@example.com");
    await setSendAppInvitation(false);
    await submitRegisterClientForm();

    await fireEvent.press(
      (await screen.findAllByRole("button", { name: translate("common.back") }))[1]!,
    );

    expect(await screen.findByText(translate("apiErrors.user.email_taken"))).toBeOnTheScreen();
    expect(registerClient).toHaveBeenCalledTimes(1);
  });
});
