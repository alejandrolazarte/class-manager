import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { ClientFeeSection } from "@/features/fees/components/ClientFeeSection";
import { deletePayment, listClientPayments } from "@/features/fees/feesApi";
import { formatMoney } from "@/features/fees/money";
import { Payment } from "@/features/fees/types";
import { translate } from "@/i18n/translate";
import { buildClient } from "@/testing/clientFactory";
import { buildCurrentMember } from "@/testing/memberFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/fees/feesApi");

const memberUserId = "user-owner";
const member = buildCurrentMember({ userId: memberUserId });
const client = buildClient();
const payment: Payment = {
  id: "payment-1",
  clientId: client.id,
  amount: 12000,
  month: "2026-09",
  paidOn: "2026-09-10",
  method: "Cash",
  notes: null,
  recordedByUserId: memberUserId,
  recordedByFullName: null,
};

describe("When a payment is deleted", () => {
  beforeEach(() => {
    jest.mocked(listClientPayments).mockResolvedValue([payment]);
    jest.mocked(deletePayment).mockResolvedValue(undefined);
  });

  it("Then it is deleted at once", async () => {
    await renderWithProviders(<ClientFeeSection client={client} />, { member });

    await fireEvent.press(
      await screen.findByRole("button", {
        name: translate("fees.client.deletePaymentOf", {
          amount: formatMoney(payment.amount, "ARS"),
        }),
      }),
    );

    await waitFor(() => expect(deletePayment).toHaveBeenCalledWith(payment.id));
  });
});
