import { waitFor } from "@testing-library/react-native";
import { registerClient } from "@/features/clients/clientsApi";
import { RegisterClientScreen } from "@/features/clients/screens/RegisterClientScreen";
import { buildClient } from "@/testing/clientFactory";
import { fillRegisterClientForm, submitRegisterClientForm } from "@/testing/fillRegisterClientForm";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/clients/clientsApi");

describe("When an adult contact who attends is registered", () => {
  beforeEach(() => {
    jest.mocked(registerClient).mockResolvedValue(buildClient());
  });

  it("Then request has the birth date", async () => {
    await renderWithProviders(<RegisterClientScreen />);
    await fillRegisterClientForm("Ana Pérez", "11 2233-4455", "20/06/1985");

    await submitRegisterClientForm();

    await waitFor(() =>
      expect(jest.mocked(registerClient).mock.calls[0]?.[0].students[0]?.birthDate).toBe(
        "1985-06-20",
      ),
    );
  });
});
