import { screen } from "@testing-library/react-native";
import { registerClient } from "@/features/clients/clientsApi";
import { RegisterClientScreen } from "@/features/clients/screens/RegisterClientScreen";
import { translate } from "@/i18n/translate";
import { fillRegisterClientForm, submitRegisterClientForm } from "@/testing/fillRegisterClientForm";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/clients/clientsApi");

const fifteenYearsAgo = new Date().getFullYear() - 15;

describe("When a contact who attends is under 18", () => {
  it("Then registration is blocked", async () => {
    await renderWithProviders(<RegisterClientScreen />);
    await fillRegisterClientForm("Juan Chico", "11 4444-1111", `10/03/${fifteenYearsAgo}`);

    await submitRegisterClientForm();

    expect(
      await screen.findByText(
        translate("clients.validation.contactMustBeAdult", { name: "Juan Chico" }),
      ),
    ).toBeOnTheScreen();
    expect(registerClient).not.toHaveBeenCalled();
  });
});
