import { screen } from "@testing-library/react-native";
import { getClient } from "@/features/clients/clientsApi";
import { AddStudentScreen } from "@/features/students/screens/AddStudentScreen";
import { translate } from "@/i18n/translate";
import { buildClient } from "@/testing/clientFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/clients/clientsApi");

const client = buildClient();

describe("When adding student to a client", () => {
  beforeEach(() => {
    jest.mocked(getClient).mockResolvedValue(client);
  });

  it("Then client name is shown", async () => {
    await renderWithProviders(<AddStudentScreen clientId={client.id} />);

    expect(
      await screen.findByText(translate("students.add.forClient", { name: client.fullName })),
    ).toBeOnTheScreen();
  });
});
