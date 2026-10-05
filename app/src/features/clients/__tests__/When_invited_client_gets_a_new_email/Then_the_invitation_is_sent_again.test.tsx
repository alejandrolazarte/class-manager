import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { getClient, updateClient } from "@/features/clients/clientsApi";
import { EditClientScreen } from "@/features/clients/screens/EditClientScreen";
import { inviteStudentApp } from "@/features/studentApp/studentAppApi";
import { translate } from "@/i18n/translate";
import { buildClient } from "@/testing/clientFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/clients/clientsApi");
jest.mock("@/features/studentApp/studentAppApi");

const client = buildClient({
  email: "ana@example.com",
  appAccess: { status: "Invited", invitedEmail: "ana@example.com" },
});

describe("When invited client gets a new email", () => {
  beforeEach(() => {
    jest.mocked(getClient).mockResolvedValue(client);
    jest.mocked(updateClient).mockResolvedValue(client);
    jest.mocked(inviteStudentApp).mockResolvedValue({
      id: "invitation-2",
      email: "ana.nueva@example.com",
      expiresAt: "2026-10-06T12:00:00Z",
    });
  });

  it("Then the invitation is sent again", async () => {
    await renderWithProviders(<EditClientScreen clientId={client.id} />);
    await fireEvent.changeText(
      await screen.findByLabelText(translate("clients.detail.email")),
      "ana.nueva@example.com",
    );

    await fireEvent.press(screen.getByRole("button", { name: translate("clients.edit.submit") }));

    await waitFor(() =>
      expect(inviteStudentApp).toHaveBeenCalledWith(client.id, {
        email: "ana.nueva@example.com",
      }),
    );
  });
});
