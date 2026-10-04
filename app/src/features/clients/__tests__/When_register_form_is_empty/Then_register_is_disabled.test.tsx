import { screen } from "@testing-library/react-native";
import { RegisterClientScreen } from "@/features/clients/screens/RegisterClientScreen";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/clients/clientsApi");

describe("When register form is empty", () => {
  it("Then register is disabled", async () => {
    await renderWithProviders(<RegisterClientScreen />);

    expect(
      screen.getByRole("button", { name: translate("clients.register.submit") }),
    ).toBeDisabled();
  });
});
