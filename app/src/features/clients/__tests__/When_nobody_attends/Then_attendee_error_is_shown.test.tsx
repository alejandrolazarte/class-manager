import { screen } from "@testing-library/react-native";
import { registerClient } from "@/features/clients/clientsApi";
import { RegisterClientScreen } from "@/features/clients/screens/RegisterClientScreen";
import { translate } from "@/i18n/translate";
import {
  fillRegisterClientForm,
  setClientAttends,
  submitRegisterClientForm,
} from "@/testing/fillRegisterClientForm";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/clients/clientsApi");

describe("When nobody attends", () => {
  it("Then attendee error is shown", async () => {
    await renderWithProviders(<RegisterClientScreen />);
    await fillRegisterClientForm("Ana Pérez", "11 2233-4455");
    await setClientAttends(false);
    await submitRegisterClientForm();

    expect(
      await screen.findByText(translate("clients.validation.attendeeRequired")),
    ).toBeOnTheScreen();
    expect(registerClient).not.toHaveBeenCalled();
  });
});
