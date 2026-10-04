import { fireEvent, screen } from "@testing-library/react-native";
import { getClassBalance } from "@/features/classPacks/classPacksApi";
import { getClient } from "@/features/clients/clientsApi";
import { ClientBillingPlanScreen } from "@/features/fees/screens/ClientBillingPlanScreen";
import { translate } from "@/i18n/translate";
import { buildClassBalance } from "@/testing/classPackFactory";
import { buildClient } from "@/testing/clientFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/clients/clientsApi");
jest.mock("@/features/fees/feesApi");
jest.mock("@/features/classPacks/classPacksApi");

const client = buildClient();

describe("When client switches to an own fee without amount", () => {
  beforeEach(() => {
    jest.mocked(getClient).mockResolvedValue(client);
    jest.mocked(getClassBalance).mockResolvedValue(buildClassBalance({ purchases: [] }));
  });

  it("Then save stays disabled", async () => {
    await renderWithProviders(<ClientBillingPlanScreen clientId={client.id} />);

    await fireEvent.press(
      await screen.findByRole("radio", { name: translate("fees.plan.CustomFee") }),
    );

    expect(screen.getByRole("button", { name: translate("common.save") })).toBeDisabled();
  });
});
