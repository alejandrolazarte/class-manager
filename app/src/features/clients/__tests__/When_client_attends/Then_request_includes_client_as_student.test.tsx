import { waitFor } from "@testing-library/react-native";
import { registerClient } from "@/features/clients/clientsApi";
import { RegisterClientScreen } from "@/features/clients/screens/RegisterClientScreen";
import { buildClient } from "@/testing/clientFactory";
import { fillRegisterClientForm, submitRegisterClientForm } from "@/testing/fillRegisterClientForm";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/clients/clientsApi");

describe("When client attends", () => {
  beforeEach(() => {
    jest.mocked(registerClient).mockResolvedValue(buildClient());
  });

  it("Then request includes client as student", async () => {
    await renderWithProviders(<RegisterClientScreen />);
    await fillRegisterClientForm("Ana Pérez", "11 2233-4455");
    await submitRegisterClientForm();

    await waitFor(() =>
      expect(registerClient).toHaveBeenCalledWith({
        fullName: "Ana Pérez",
        phoneNumber: "11 2233-4455",
        students: [{ fullName: "Ana Pérez", birthDate: null, notes: null, email: null }],
      }),
    );
  });
});
