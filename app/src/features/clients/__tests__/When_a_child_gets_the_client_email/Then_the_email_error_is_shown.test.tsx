import { fireEvent, screen } from "@testing-library/react-native";
import { registerClient } from "@/features/clients/clientsApi";
import { RegisterClientScreen } from "@/features/clients/screens/RegisterClientScreen";
import { translate } from "@/i18n/translate";
import {
  addAdditionalStudent,
  fillRegisterClientEmail,
  fillRegisterClientForm,
  submitRegisterClientForm,
} from "@/testing/fillRegisterClientForm";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/clients/clientsApi");

describe("When a child gets the client email", () => {
  it("Then the email error is shown", async () => {
    await renderWithProviders(<RegisterClientScreen />);
    await fillRegisterClientForm("Ana Pérez", "11 2233-4455");
    await fillRegisterClientEmail("ana@example.com");
    await addAdditionalStudent("Tomás Pérez");
    await fireEvent.changeText(
      screen.getByLabelText(translate("students.fields.email")),
      "ana@example.com",
    );

    await submitRegisterClientForm();

    expect(
      await screen.findByText(translate("students.validation.emailOfAnotherPerson")),
    ).toBeOnTheScreen();
    expect(registerClient).not.toHaveBeenCalled();
  });
});
