import { screen, waitFor } from "@testing-library/react-native";
import { registerClient } from "@/features/clients/clientsApi";
import { RegisterClientScreen } from "@/features/clients/screens/RegisterClientScreen";
import { translate } from "@/i18n/translate";
import { buildClient } from "@/testing/clientFactory";
import { routerMock } from "@/testing/expoRouterMock";
import { fillRegisterClientForm, submitRegisterClientForm } from "@/testing/fillRegisterClientForm";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/clients/clientsApi");

const registeredClient = buildClient();

describe("When registration succeeds", () => {
  beforeEach(() => {
    jest.mocked(registerClient).mockResolvedValue(registeredClient);
  });

  it("Then navigates to client detail", async () => {
    await renderWithProviders(<RegisterClientScreen />);
    await fillRegisterClientForm(registeredClient.fullName, "11 2233-4455");
    await submitRegisterClientForm();

    await waitFor(() =>
      expect(routerMock.replace).toHaveBeenCalledWith(`/students/clients/${registeredClient.id}`),
    );
    expect(screen.getByText(translate("clients.register.success"))).toBeOnTheScreen();
  });
});
