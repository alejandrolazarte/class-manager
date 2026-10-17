import { screen } from "@testing-library/react-native";
import { RegisterClientScreen } from "@/features/clients/screens/RegisterClientScreen";
import { translate } from "@/i18n/translate";
import { fillRegisterClientEmail, fillRegisterClientForm } from "@/testing/fillRegisterClientForm";
import { confirmContactIsAdult } from "@/testing/confirmContactIsAdult";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/clients/clientsApi");
jest.mock("@/features/studentApp/studentAppApi");

describe("When the app invitation is on", () => {
  it("Then register waits for the adult confirmation", async () => {
    await renderWithProviders(<RegisterClientScreen />);
    await fillRegisterClientForm("Ana Pérez", "11 2233-4455");
    await fillRegisterClientEmail("ana@example.com");
    const registerButton = screen.getByRole("button", {
      name: translate("clients.register.submit"),
    });
    expect(registerButton).toBeDisabled();

    await confirmContactIsAdult("Ana Pérez");

    expect(
      screen.getByRole("button", { name: translate("clients.register.submit") }),
    ).toBeEnabled();
  });
});
