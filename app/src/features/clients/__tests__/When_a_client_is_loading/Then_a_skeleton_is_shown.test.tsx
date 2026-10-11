import { screen } from "@testing-library/react-native";
import { getClient } from "@/features/clients/clientsApi";
import { ClientDetailScreen } from "@/features/clients/screens/ClientDetailScreen";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/clients/clientsApi");

describe("When a client is loading", () => {
  beforeEach(() => {
    jest.mocked(getClient).mockImplementation(() => new Promise(() => undefined));
  });

  it("Then a skeleton is shown", async () => {
    await renderWithProviders(<ClientDetailScreen clientId="client-loading" />);

    expect(
      screen.getByRole("progressbar", { name: translate("common.loading") }),
    ).toBeOnTheScreen();
  });
});
