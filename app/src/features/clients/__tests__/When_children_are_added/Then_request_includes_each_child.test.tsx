import { waitFor } from "@testing-library/react-native";
import { registerClient } from "@/features/clients/clientsApi";
import { RegisterClientScreen } from "@/features/clients/screens/RegisterClientScreen";
import { buildClient } from "@/testing/clientFactory";
import {
  addAdditionalStudent,
  fillRegisterClientForm,
  setClientAttends,
  submitRegisterClientForm,
} from "@/testing/fillRegisterClientForm";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/clients/clientsApi");

describe("When children are added", () => {
  beforeEach(() => {
    jest.mocked(registerClient).mockResolvedValue(buildClient());
  });

  it("Then request includes each child", async () => {
    await renderWithProviders(<RegisterClientScreen />);
    await fillRegisterClientForm("Ana Pérez", "11 2233-4455");
    await setClientAttends(false);
    await addAdditionalStudent("Tomás Pérez", "14032018");
    await addAdditionalStudent("Lucía Pérez");
    await submitRegisterClientForm();

    await waitFor(() =>
      expect(jest.mocked(registerClient).mock.calls[0]?.[0].students).toEqual([
        { fullName: "Tomás Pérez", birthDate: "2018-03-14", notes: null },
        { fullName: "Lucía Pérez", birthDate: null, notes: null },
      ]),
    );
  });
});
