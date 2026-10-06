import { fireEvent, screen } from "@testing-library/react-native";
import { ClientFeeSection } from "@/features/fees/components/ClientFeeSection";
import { deleteClientBillingPlanChange, listClientPayments } from "@/features/fees/feesApi";
import { addMonths, formatMonth, monthOf } from "@/features/fees/months";
import { translate, translateCount } from "@/i18n/translate";
import { buildClient } from "@/testing/clientFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/fees/feesApi");

const upcomingMonth = addMonths(monthOf(), 1);
const client = buildClient({
  billingPlanChanges: [{ effectiveFrom: upcomingMonth, kind: "ClassPacks", customFee: null }],
});

describe("When upcoming client fee change deletion is cancelled", () => {
  beforeEach(() => {
    jest.mocked(listClientPayments).mockResolvedValue([]);
  });

  it("Then nothing is deleted", async () => {
    await renderWithProviders(<ClientFeeSection client={client} />);

    await fireEvent.press(screen.getByText(translateCount("fees.client.upcomingChanges", 1)));
    await fireEvent.press(
      screen.getByRole("button", {
        name: translate("fees.client.deleteUpcomingChange", { month: formatMonth(upcomingMonth) }),
      }),
    );
    await fireEvent.press(screen.getByRole("button", { name: translate("common.cancel") }));

    expect(screen.queryByRole("button", { name: translate("common.confirmDelete") })).toBeNull();
    expect(deleteClientBillingPlanChange).not.toHaveBeenCalled();
  });
});
