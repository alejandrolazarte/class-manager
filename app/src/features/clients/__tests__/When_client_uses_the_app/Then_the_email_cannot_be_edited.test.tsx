import { screen } from "@testing-library/react-native";
import { getClient } from "@/features/clients/clientsApi";
import { EditClientScreen } from "@/features/clients/screens/EditClientScreen";
import { translate } from "@/i18n/translate";
import { buildClient } from "@/testing/clientFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/clients/clientsApi");

const client = buildClient({
  email: "ana.vieja@example.com",
  appAccess: { status: "Active", invitedEmail: null, signInEmail: "ana@example.com" },
});

describe("When client uses the app", () => {
  beforeEach(() => {
    jest.mocked(getClient).mockResolvedValue(client);
  });

  it("Then the email cannot be edited", async () => {
    await renderWithProviders(<EditClientScreen clientId={client.id} />);

    const emailField = await screen.findByLabelText(translate("clients.detail.email"));

    expect(emailField).toHaveProp("editable", false);
    expect(emailField).toHaveDisplayValue("ana@example.com");
  });
});
