import { screen } from "@testing-library/react-native";
import { RegisterClientScreen } from "@/features/clients/screens/RegisterClientScreen";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/clients/clientsApi");

describe("When email is empty", () => {
  it("Then invitation option is hidden", async () => {
    await renderWithProviders(<RegisterClientScreen />);

    expect(screen.getByLabelText(translate("clients.register.email"))).toBeOnTheScreen();
    expect(
      screen.queryByRole("switch", { name: translate("clients.register.sendAppInvitation") }),
    ).not.toBeOnTheScreen();
  });
});
