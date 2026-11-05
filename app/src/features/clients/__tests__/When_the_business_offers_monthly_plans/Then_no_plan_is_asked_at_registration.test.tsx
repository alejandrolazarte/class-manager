import { screen, waitFor } from "@testing-library/react-native";
import { listClassPacks } from "@/features/classPacks/classPacksApi";
import { registerClient } from "@/features/clients/clientsApi";
import { RegisterClientScreen } from "@/features/clients/screens/RegisterClientScreen";
import { buildClient } from "@/testing/clientFactory";
import { fillRegisterClientForm, submitRegisterClientForm } from "@/testing/fillRegisterClientForm";
import { buildMonthlyPlan } from "@/testing/monthlyPlanFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/clients/clientsApi");
jest.mock("@/features/classPacks/classPacksApi");

const plan = buildMonthlyPlan({ name: "Libre" });

describe("When the business offers monthly plans", () => {
  beforeEach(() => {
    jest.mocked(listClassPacks).mockResolvedValue([plan]);
    jest.mocked(registerClient).mockResolvedValue(buildClient());
  });

  it("Then no plan is asked at registration", async () => {
    await renderWithProviders(<RegisterClientScreen />);
    await fillRegisterClientForm("Ana Pérez", "11 2233-4455");

    expect(screen.queryByText(plan.name)).toBeNull();
    await submitRegisterClientForm();
    await waitFor(() => expect(registerClient).toHaveBeenCalled());
    expect(jest.mocked(registerClient).mock.calls[0]?.[0]).not.toHaveProperty("planId");
  });
});
