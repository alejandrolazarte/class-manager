import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { listClassPacks } from "@/features/classPacks/classPacksApi";
import { registerClient } from "@/features/clients/clientsApi";
import { RegisterClientScreen } from "@/features/clients/screens/RegisterClientScreen";
import { buildClient } from "@/testing/clientFactory";
import { fillRegisterClientForm, submitRegisterClientForm } from "@/testing/fillRegisterClientForm";
import { buildMonthlyPlan } from "@/testing/monthlyPlanFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/clients/clientsApi");
jest.mock("@/features/classPacks/classPacksApi");

const libre = buildMonthlyPlan({ id: "0192f0c4-0000-7000-8000-00000000c1a7", name: "Libre" });

describe("When the business offers monthly plans", () => {
  beforeEach(() => {
    jest.mocked(listClassPacks).mockResolvedValue([buildMonthlyPlan(), libre]);
    jest.mocked(registerClient).mockResolvedValue(buildClient());
  });

  it("Then the chosen plan is sent", async () => {
    await renderWithProviders(<RegisterClientScreen />);
    await fillRegisterClientForm("Ana Pérez", "11 2233-4455");
    await fireEvent.press(await screen.findByText("Libre"));
    await submitRegisterClientForm();

    await waitFor(() =>
      expect(registerClient).toHaveBeenCalledWith(expect.objectContaining({ planId: libre.id })),
    );
  });
});
