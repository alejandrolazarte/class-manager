import { screen } from "@testing-library/react-native";
import { registerClient } from "@/features/clients/clientsApi";
import { RegisterClientScreen } from "@/features/clients/screens/RegisterClientScreen";
import { translate } from "@/i18n/translate";
import { fillRegisterClientForm, submitRegisterClientForm } from "@/testing/fillRegisterClientForm";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/clients/clientsApi");

describe("When register name is too short", () => {
  it("Then the name error is shown", async () => {
    await renderWithProviders(<RegisterClientScreen />);
    await fillRegisterClientForm("A", "11 2233-4455");

    await submitRegisterClientForm();

    expect(
      await screen.findByText(translate("clients.validation.fullNameTooShort")),
    ).toBeOnTheScreen();
    expect(registerClient).not.toHaveBeenCalled();
  });
});
