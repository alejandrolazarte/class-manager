import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { ApiError } from "@/api/httpClient";
import { registerClient } from "@/features/clients/clientsApi";
import { RegisterClientScreen } from "@/features/clients/screens/RegisterClientScreen";
import { userErrorCodes } from "@/features/users/userErrorCodes";
import { translate } from "@/i18n/translate";
import { buildClient } from "@/testing/clientFactory";
import {
  fillRegisterClientEmail,
  fillRegisterClientForm,
  setSendAppInvitation,
  submitRegisterClientForm,
} from "@/testing/fillRegisterClientForm";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/clients/clientsApi");

const conflictStatus = 409;
const instructorUserId = "user-of-the-instructor";

describe("When a client is registered with the email of an instructor", () => {
  beforeEach(() => {
    jest
      .mocked(registerClient)
      .mockRejectedValueOnce(
        new ApiError(conflictStatus, {
          code: userErrorCodes.samePerson,
          samePersonUserId: instructorUserId,
          samePersonFullName: "Ana Gómez",
          samePersonField: "Email",
        }),
      )
      .mockResolvedValueOnce(buildClient());
  });

  it("Then it is registered after confirming the same person", async () => {
    await renderWithProviders(<RegisterClientScreen />);
    await fillRegisterClientForm("Ana Gómez", "11 2233-4455");
    await fillRegisterClientEmail("ana@example.com");
    await setSendAppInvitation(false);
    await submitRegisterClientForm();
    expect(
      await screen.findByText(translate("people.samePerson.question", { name: "Ana Gómez" })),
    ).toBeOnTheScreen();
    expect(registerClient).toHaveBeenCalledTimes(1);

    await fireEvent.press(
      screen.getByRole("button", { name: translate("people.samePerson.confirm") }),
    );

    await waitFor(() =>
      expect(jest.mocked(registerClient).mock.calls[1]?.[0]).toEqual(
        expect.objectContaining({ email: "ana@example.com", samePersonUserId: instructorUserId }),
      ),
    );
  });
});
