import { screen } from "@testing-library/react-native";
import { RegisterClientScreen } from "@/features/clients/screens/RegisterClientScreen";
import { translate } from "@/i18n/translate";
import { buildBusiness } from "@/testing/businessFactory";
import { fillRegisterClientForm } from "@/testing/fillRegisterClientForm";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/clients/clientsApi");

describe("When the business has not chosen how it charges", () => {
  it("Then register stays disabled until it is chosen", async () => {
    await renderWithProviders(<RegisterClientScreen />, {
      business: buildBusiness({ defaultMonthlyFeeChanges: [], billingMode: null }),
    });
    await fillRegisterClientForm("Ana Pérez", "11 2233-4455");

    expect(
      screen.getByRole("button", { name: translate("clients.register.submit") }),
    ).toBeDisabled();
  });
});
