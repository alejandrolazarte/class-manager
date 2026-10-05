import { screen } from "@testing-library/react-native";
import { getClassBalance } from "@/features/classPacks/classPacksApi";
import { getClient } from "@/features/clients/clientsApi";
import { ClientBillingPlanScreen } from "@/features/fees/screens/ClientBillingPlanScreen";
import { translateCount } from "@/i18n/translate";
import { buildClassBalance } from "@/testing/classPackFactory";
import { buildClient } from "@/testing/clientFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/clients/clientsApi");
jest.mock("@/features/fees/feesApi");
jest.mock("@/features/classPacks/classPacksApi");

const client = buildClient();

describe("When client keeps bought classes with a monthly fee", () => {
  beforeEach(() => {
    jest.mocked(getClient).mockResolvedValue(client);
    jest.mocked(getClassBalance).mockResolvedValue(buildClassBalance({ availableClasses: 19 }));
  });

  it("Then the classes are said to be kept", async () => {
    await renderWithProviders(<ClientBillingPlanScreen clientId={client.id} />);

    expect(
      await screen.findByText(translateCount("fees.billingPlan.keepClasses", 19)),
    ).toBeOnTheScreen();
  });
});
