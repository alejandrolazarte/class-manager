import { fireEvent, screen } from "@testing-library/react-native";
import { getClient } from "@/features/clients/clientsApi";
import { EditClientScreen } from "@/features/clients/screens/EditClientScreen";
import { translate } from "@/i18n/translate";
import { buildClient } from "@/testing/clientFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/clients/clientsApi");

const client = buildClient();

describe("When edited client has no name", () => {
  beforeEach(() => {
    jest.mocked(getClient).mockResolvedValue(client);
  });

  it("Then save is disabled", async () => {
    await renderWithProviders(<EditClientScreen clientId={client.id} />);

    await fireEvent.changeText(
      await screen.findByLabelText(translate("clients.register.fullName")),
      "  ",
    );

    expect(screen.getByRole("button", { name: translate("clients.edit.submit") })).toBeDisabled();
  });
});
