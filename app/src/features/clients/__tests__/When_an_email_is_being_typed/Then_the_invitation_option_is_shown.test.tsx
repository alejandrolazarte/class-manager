import { screen } from "@testing-library/react-native";
import { RegisterClientScreen } from "@/features/clients/screens/RegisterClientScreen";
import { translate } from "@/i18n/translate";
import { fillRegisterClientEmail } from "@/testing/fillRegisterClientForm";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/clients/clientsApi");

describe("When an email is being typed", () => {
  it("Then the invitation option is shown", async () => {
    await renderWithProviders(<RegisterClientScreen />);

    await fillRegisterClientEmail("ana@");

    expect(
      screen.getByRole("switch", { name: translate("clients.register.sendAppInvitation") }),
    ).toBeOnTheScreen();
  });
});
