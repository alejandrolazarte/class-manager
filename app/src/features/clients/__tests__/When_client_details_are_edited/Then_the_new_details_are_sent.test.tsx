import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { getClient, updateClient } from "@/features/clients/clientsApi";
import { EditClientScreen } from "@/features/clients/screens/EditClientScreen";
import { translate } from "@/i18n/translate";
import { buildClient } from "@/testing/clientFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/clients/clientsApi");

const client = buildClient({ email: null, notes: null });

describe("When client details are edited", () => {
  beforeEach(() => {
    jest.mocked(getClient).mockResolvedValue(client);
    jest.mocked(updateClient).mockResolvedValue(client);
  });

  it("Then the new details are sent", async () => {
    await renderWithProviders(<EditClientScreen clientId={client.id} />);
    await fireEvent.changeText(
      await screen.findByLabelText(translate("clients.register.fullName")),
      "Ana María Pérez",
    );
    await fireEvent.changeText(
      screen.getByLabelText(translate("clients.detail.email")),
      "ana@example.com",
    );

    await fireEvent.press(screen.getByRole("button", { name: translate("clients.edit.submit") }));

    await waitFor(() =>
      expect(updateClient).toHaveBeenCalledWith(client.id, {
        fullName: "Ana María Pérez",
        phoneNumber: "+54 9 11 2233-4455",
        email: "ana@example.com",
        notes: null,
      }),
    );
  });
});
