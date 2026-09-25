import { fireEvent, screen } from "@testing-library/react-native";
import { registerClient } from "@/features/clients/clientsApi";
import { RegisterClientScreen } from "@/features/clients/screens/RegisterClientScreen";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/clients/clientsApi");

describe("When register form is submitted empty", () => {
  it("Then name and phone errors are shown", async () => {
    await renderWithProviders(<RegisterClientScreen />);

    await fireEvent.press(
      screen.getByRole("button", { name: translate("clients.register.submit") }),
    );

    expect(
      await screen.findByText(translate("clients.validation.fullNameRequired")),
    ).toBeOnTheScreen();
    expect(screen.getByText(translate("clients.validation.phoneNumberRequired"))).toBeOnTheScreen();
    expect(registerClient).not.toHaveBeenCalled();
  });
});
