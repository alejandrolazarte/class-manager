import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { listMonthlyFees, recordPayment } from "@/features/fees/feesApi";
import { RecordPaymentScreen } from "@/features/fees/screens/RecordPaymentScreen";
import { translate } from "@/i18n/translate";
import { buildClientFee, buildMonthlyFees, feeMonth } from "@/testing/feeFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/fees/feesApi");

const clientFee = buildClientFee();

describe("When payment is recorded", () => {
  beforeEach(() => {
    jest.mocked(listMonthlyFees).mockResolvedValue(buildMonthlyFees([clientFee]));
    jest.mocked(recordPayment).mockResolvedValue({
      id: "payment",
      clientId: clientFee.clientId,
      amount: 7000,
      month: feeMonth,
      paidOn: "2026-09-05",
      method: "Transfer",
      notes: null,
    });
  });

  it("Then request has amount month and method", async () => {
    await renderWithProviders(
      <RecordPaymentScreen clientId={clientFee.clientId} month={feeMonth} />,
    );
    await screen.findByLabelText(translate("fees.payment.amount"));

    await fireEvent.press(screen.getByRole("button", { name: translate("fees.methods.Transfer") }));
    await fireEvent.press(screen.getByRole("button", { name: translate("fees.payment.submit") }));

    await waitFor(() =>
      expect(recordPayment).toHaveBeenCalledWith(
        clientFee.clientId,
        expect.objectContaining({ amount: 7000, month: feeMonth, method: "Transfer" }),
      ),
    );
  });
});
