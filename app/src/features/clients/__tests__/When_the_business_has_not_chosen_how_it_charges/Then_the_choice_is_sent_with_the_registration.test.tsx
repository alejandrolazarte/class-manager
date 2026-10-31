import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { registerClient } from "@/features/clients/clientsApi";
import { RegisterClientScreen } from "@/features/clients/screens/RegisterClientScreen";
import { translate } from "@/i18n/translate";
import { buildBusiness } from "@/testing/businessFactory";
import { buildClient } from "@/testing/clientFactory";
import { fillRegisterClientForm, submitRegisterClientForm } from "@/testing/fillRegisterClientForm";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/clients/clientsApi");

describe("When the business has not chosen how it charges", () => {
  beforeEach(() => {
    jest.mocked(registerClient).mockResolvedValue(buildClient());
  });

  it("Then the choice is sent with the registration", async () => {
    await renderWithProviders(<RegisterClientScreen />, {
      business: buildBusiness({ defaultMonthlyFeeChanges: [], billingMode: null }),
    });
    await fillRegisterClientForm("Ana Pérez", "11 2233-4455");
    await fireEvent.press(screen.getByText(translate("fees.billingMode.ClassPacks")));
    await submitRegisterClientForm();

    await waitFor(() =>
      expect(registerClient).toHaveBeenCalledWith(
        expect.objectContaining({ billingMode: "ClassPacks" }),
      ),
    );
  });
});
