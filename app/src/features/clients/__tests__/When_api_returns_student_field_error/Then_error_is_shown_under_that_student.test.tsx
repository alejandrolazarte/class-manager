import { screen } from "@testing-library/react-native";
import { ApiError } from "@/api/httpClient";
import { registerClient } from "@/features/clients/clientsApi";
import { RegisterClientScreen } from "@/features/clients/screens/RegisterClientScreen";
import {
  addAdditionalStudent,
  fillRegisterClientForm,
  submitRegisterClientForm,
} from "@/testing/fillRegisterClientForm";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/clients/clientsApi");

const badRequestStatus = 400;
const studentNameServerMessage = "Full name must be between 2 and 120 characters.";

describe("When api returns student field error", () => {
  beforeEach(() => {
    jest.mocked(registerClient).mockRejectedValue(
      new ApiError(badRequestStatus, {
        errors: { "students[1].FullName": [studentNameServerMessage] },
      }),
    );
  });

  it("Then error is shown under that student", async () => {
    await renderWithProviders(<RegisterClientScreen />);
    await fillRegisterClientForm("Ana Pérez", "11 2233-4455");
    await addAdditionalStudent("Tomás Pérez");
    await submitRegisterClientForm();

    expect(await screen.findByText(translate("apiErrors.invalidValue"))).toBeOnTheScreen();
  });
});
