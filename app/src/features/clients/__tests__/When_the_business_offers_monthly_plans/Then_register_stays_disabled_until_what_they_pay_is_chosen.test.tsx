import { screen } from "@testing-library/react-native";
import { listClassPacks } from "@/features/classPacks/classPacksApi";
import { RegisterClientScreen } from "@/features/clients/screens/RegisterClientScreen";
import { translate } from "@/i18n/translate";
import { fillRegisterClientForm } from "@/testing/fillRegisterClientForm";
import { buildMonthlyPlan } from "@/testing/monthlyPlanFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/clients/clientsApi");
jest.mock("@/features/classPacks/classPacksApi");

describe("When the business offers monthly plans", () => {
  beforeEach(() => {
    jest
      .mocked(listClassPacks)
      .mockResolvedValue([
        buildMonthlyPlan(),
        buildMonthlyPlan({ id: "0192f0c4-0000-7000-8000-00000000c1a7", name: "Libre" }),
      ]);
  });

  it("Then register stays disabled until what they pay is chosen", async () => {
    await renderWithProviders(<RegisterClientScreen />);
    await screen.findByText("Libre");
    await fillRegisterClientForm("Ana Pérez", "11 2233-4455");

    expect(
      screen.getByRole("button", { name: translate("clients.register.submit") }),
    ).toBeDisabled();
  });
});
