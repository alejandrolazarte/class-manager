import { screen } from "@testing-library/react-native";
import { RegisterClientScreen } from "@/features/clients/screens/RegisterClientScreen";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/clients/clientsApi");

describe("When the business already charges", () => {
  it("Then how it charges is not asked", async () => {
    await renderWithProviders(<RegisterClientScreen />);

    expect(screen.queryByText(translate("fees.billingMode.ClassPacks"))).toBeNull();
  });
});
