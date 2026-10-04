import { screen, waitFor } from "@testing-library/react-native";
import { registerClient } from "@/features/clients/clientsApi";
import { RegisterClientScreen } from "@/features/clients/screens/RegisterClientScreen";
import { inviteStudentApp } from "@/features/studentApp/studentAppApi";
import { translate } from "@/i18n/translate";
import { buildClient } from "@/testing/clientFactory";
import { routerMock } from "@/testing/expoRouterMock";
import {
  fillRegisterClientEmail,
  fillRegisterClientForm,
  submitRegisterClientForm,
} from "@/testing/fillRegisterClientForm";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/clients/clientsApi");
jest.mock("@/features/studentApp/studentAppApi");

const registeredClient = buildClient({ email: "ana@example.com" });

describe("When app invitation fails", () => {
  beforeEach(() => {
    jest.mocked(registerClient).mockResolvedValue(registeredClient);
    jest.mocked(inviteStudentApp).mockRejectedValue(new Error("Email delivery failed"));
  });

  it("Then client detail opens with a warning", async () => {
    await renderWithProviders(<RegisterClientScreen />);
    await fillRegisterClientForm(registeredClient.fullName, "11 2233-4455");
    await fillRegisterClientEmail("ana@example.com");
    await submitRegisterClientForm();

    expect(
      await screen.findByText(translate("clients.register.invitationFailed")),
    ).toBeOnTheScreen();
    await waitFor(() =>
      expect(routerMock.replace).toHaveBeenCalledWith(`/students/clients/${registeredClient.id}`),
    );
  });
});
