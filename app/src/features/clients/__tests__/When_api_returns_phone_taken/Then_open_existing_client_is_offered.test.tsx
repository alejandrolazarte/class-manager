import { fireEvent, screen } from "@testing-library/react-native";
import { ApiError } from "@/api/httpClient";
import { clientErrorCodes } from "@/features/clients/clientErrorCodes";
import { getClient, registerClient } from "@/features/clients/clientsApi";
import { RegisterClientScreen } from "@/features/clients/screens/RegisterClientScreen";
import { translate } from "@/i18n/translate";
import { buildClient } from "@/testing/clientFactory";
import { routerMock } from "@/testing/expoRouterMock";
import { fillRegisterClientForm, submitRegisterClientForm } from "@/testing/fillRegisterClientForm";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/clients/clientsApi");

const existingClient = buildClient({ fullName: "Marta Gómez" });
const conflictStatus = 409;

describe("When api returns phone taken", () => {
  beforeEach(() => {
    jest.mocked(registerClient).mockRejectedValue(
      new ApiError(conflictStatus, {
        code: clientErrorCodes.phoneNumberTaken,
        clientId: existingClient.id,
      }),
    );
    jest.mocked(getClient).mockResolvedValue(existingClient);
  });

  it("Then open existing client is offered", async () => {
    await renderWithProviders(<RegisterClientScreen />);
    await fillRegisterClientForm("Ana Pérez", "11 2233-4455");
    await submitRegisterClientForm();

    expect(
      await screen.findByText(
        translate("clients.register.phoneTaken", { name: existingClient.fullName }),
      ),
    ).toBeOnTheScreen();
    await fireEvent.press(
      screen.getByRole("button", { name: translate("clients.register.openClient") }),
    );
    expect(routerMock.push).toHaveBeenCalledWith(`/students/clients/${existingClient.id}`);
  });
});
