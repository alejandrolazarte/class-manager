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

describe("When email is entered", () => {
  beforeEach(() => {
    jest.mocked(registerClient).mockResolvedValue(registeredClient);
    jest.mocked(inviteStudentApp).mockResolvedValue({
      id: "invitation-1",
      email: "ana@example.com",
      expiresAt: "2026-10-10T12:00:00Z",
    });
  });

  it("Then app invitation is sent", async () => {
    await renderWithProviders(<RegisterClientScreen />);
    await fillRegisterClientForm(registeredClient.fullName, "11 2233-4455");
    await fillRegisterClientEmail("ana@example.com");
    await submitRegisterClientForm();

    await waitFor(() =>
      expect(inviteStudentApp).toHaveBeenCalledWith(registeredClient.id, {
        email: "ana@example.com",
      }),
    );
    expect(
      await screen.findByText(translate("clients.register.successWithInvitation")),
    ).toBeOnTheScreen();
    expect(routerMock.replace).toHaveBeenCalledWith(`/students/clients/${registeredClient.id}`);
  });
});
