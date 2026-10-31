import { screen, waitFor } from "@testing-library/react-native";
import { listClassPacks } from "@/features/classPacks/classPacksApi";
import { RegisterClientScreen } from "@/features/clients/screens/RegisterClientScreen";
import { translate } from "@/i18n/translate";
import { buildClassPack } from "@/testing/classPackFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/clients/clientsApi");
jest.mock("@/features/classPacks/classPacksApi");

describe("When the business offers no monthly plans", () => {
  beforeEach(() => {
    jest.mocked(listClassPacks).mockResolvedValue([buildClassPack()]);
  });

  it("Then what they pay is not asked", async () => {
    await renderWithProviders(<RegisterClientScreen />);
    await waitFor(() => expect(listClassPacks).toHaveBeenCalled());

    expect(
      screen.queryByText(translate("clients.register.planQuestion"), { exact: false }),
    ).toBeNull();
  });
});
