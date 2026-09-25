import { screen } from "@testing-library/react-native";
import { ApiError } from "@/api/httpClient";
import { registerClient } from "@/features/clients/clientsApi";
import { RegisterClientScreen } from "@/features/clients/screens/RegisterClientScreen";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { fillRegisterClientForm, submitRegisterClientForm } from "@/testing/fillRegisterClientForm";

jest.mock("@/features/clients/clientsApi");

const badRequestStatus = 400;
const phoneNumberServerMessage = "The phone number is not a valid mobile number.";

describe("When api returns validation errors", () => {
  beforeEach(() => {
    jest
      .mocked(registerClient)
      .mockRejectedValue(
        new ApiError(badRequestStatus, { errors: { PhoneNumber: [phoneNumberServerMessage] } }),
      );
  });

  it("Then errors are mapped to fields", async () => {
    await renderWithProviders(<RegisterClientScreen />);
    await fillRegisterClientForm("Ana Pérez", "11 2233-4455");
    await submitRegisterClientForm();

    expect(await screen.findByText(phoneNumberServerMessage)).toBeOnTheScreen();
  });
});
