import { screen } from "@testing-library/react-native";
import { ClientFeeSection } from "@/features/fees/components/ClientFeeSection";
import { listClientPayments } from "@/features/fees/feesApi";
import { formatMoney } from "@/features/fees/money";
import { Payment } from "@/features/fees/types";
import { translate } from "@/i18n/translate";
import { buildClient } from "@/testing/clientFactory";
import { buildCurrentMember } from "@/testing/memberFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/fees/feesApi");

const collectorUserId = "user-collector";
const client = buildClient();
const basePayment: Payment = {
  id: "payment-own",
  clientId: client.id,
  amount: 12000,
  month: "2026-09",
  paidOn: "2026-09-10",
  method: "Cash",
  notes: null,
  recordedByUserId: collectorUserId,
  recordedByFullName: "Marcos Díaz",
};
const ownerPayment: Payment = {
  ...basePayment,
  id: "payment-owner",
  amount: 5000,
  recordedByUserId: "user-owner",
  recordedByFullName: "Ana Pérez",
};

describe("When collector opens a family", () => {
  beforeEach(() => {
    jest.mocked(listClientPayments).mockResolvedValue([basePayment, ownerPayment]);
  });

  it("Then only their payments can be deleted", async () => {
    await renderWithProviders(<ClientFeeSection client={client} />, {
      member: buildCurrentMember({
        userId: collectorUserId,
        branchRole: "Custom",
        isBrandOwner: false,
        permissions: ["payments.view.own", "payments.record"],
      }),
    });

    expect(
      await screen.findByText(translate("fees.client.recordedBy", { name: "Ana Pérez" }), {
        exact: false,
      }),
    ).toBeOnTheScreen();
    expect(
      screen.getByRole("button", {
        name: translate("fees.client.deletePaymentOf", {
          amount: formatMoney(basePayment.amount, "ARS"),
        }),
      }),
    ).toBeOnTheScreen();
    expect(
      screen.queryByRole("button", {
        name: translate("fees.client.deletePaymentOf", {
          amount: formatMoney(ownerPayment.amount, "ARS"),
        }),
      }),
    ).toBeNull();
  });
});
