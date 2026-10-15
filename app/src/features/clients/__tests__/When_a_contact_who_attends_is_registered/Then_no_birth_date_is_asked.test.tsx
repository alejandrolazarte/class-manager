import { screen } from "@testing-library/react-native";
import { RegisterClientScreen } from "@/features/clients/screens/RegisterClientScreen";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/clients/clientsApi");

describe("When a contact who attends is registered", () => {
  it("Then no birth date is asked", async () => {
    await renderWithProviders(<RegisterClientScreen />);

    expect(
      await screen.findByRole("switch", { name: translate("clients.register.clientAttends") }),
    ).toBeOnTheScreen();
    expect(screen.queryByLabelText(translate("birthDate.label"))).toBeNull();
  });
});
