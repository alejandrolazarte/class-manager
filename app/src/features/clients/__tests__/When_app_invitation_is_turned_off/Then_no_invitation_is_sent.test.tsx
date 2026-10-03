import { screen, waitFor } from "@testing-library/react-native";
import { registerClient } from "@/features/clients/clientsApi";
import { RegisterClientScreen } from "@/features/clients/screens/RegisterClientScreen";
import { inviteFamily } from "@/features/family/familyApi";
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
jest.mock("@/features/family/familyApi");

const registeredClient = buildClient({ email: "ana@example.com" });

describe("When app invitation is turned off", () => {
  beforeEach(() => {
    jest.mocked(registerClient).mockResolvedValue(registeredClient);
  });

  it("Then no invitation is sent", async () => {
    await renderWithProviders(<RegisterClientScreen />);
    await fillRegisterClientForm(registeredClient.fullName, "11 2233-4455");
    await fillRegisterClientEmail("ana@example.com");
    await setSendAppInvitation(false);
    await submitRegisterClientForm();

    expect(await screen.findByText(translate("clients.register.success"))).toBeOnTheScreen();
    await waitFor(() =>
      expect(registerClient).toHaveBeenCalledWith(
        expect.objectContaining({ email: "ana@example.com" }),
      ),
    );
    expect(inviteFamily).not.toHaveBeenCalled();
  });
});
