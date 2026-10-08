import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { ClientFeeSection } from "@/features/fees/components/ClientFeeSection";
import { deleteClientBillingPlanChange, listClientPayments } from "@/features/fees/feesApi";
import { addMonths, formatMonth, monthOf } from "@/features/fees/months";
import { translate, translateCount } from "@/i18n/translate";
import { buildClient } from "@/testing/clientFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/fees/feesApi");

const upcomingMonth = addMonths(monthOf(), 2);
const client = buildClient({
  billingPlanChanges: [{ effectiveFrom: upcomingMonth, kind: "CustomFee", customFee: 30 }],
});

describe("When upcoming client fee change is deleted", () => {
  beforeEach(() => {
    jest.mocked(listClientPayments).mockResolvedValue([]);
    jest.mocked(deleteClientBillingPlanChange).mockResolvedValue({
      billingPlan: client.billingPlan,
      billingPlanChanges: [],
    });
  });

  it("Then it is deleted at once", async () => {
    await renderWithProviders(<ClientFeeSection client={client} />);

    await fireEvent.press(screen.getByText(translateCount("fees.client.upcomingChanges", 1)));
    await fireEvent.press(
      screen.getByRole("button", {
        name: translate("fees.client.deleteUpcomingChange", { month: formatMonth(upcomingMonth) }),
      }),
    );

    await waitFor(() =>
      expect(deleteClientBillingPlanChange).toHaveBeenCalledWith(client.id, upcomingMonth),
    );
  });
});
