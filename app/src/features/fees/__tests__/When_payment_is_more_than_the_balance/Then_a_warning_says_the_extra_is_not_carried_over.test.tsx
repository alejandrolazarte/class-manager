import { fireEvent, screen } from "@testing-library/react-native";
import { listMonthlyFees } from "@/features/fees/feesApi";
import { formatMoney } from "@/features/fees/money";
import { RecordPaymentScreen } from "@/features/fees/screens/RecordPaymentScreen";
import { translate } from "@/i18n/translate";
import { buildClientFee, buildMonthlyFees, feeMonth } from "@/testing/feeFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/fees/feesApi");

const clientFee = buildClientFee({ fee: 12000, paid: 5000, balance: 7000 });
const currencyCode = "ARS";

describe("When payment is more than the balance", () => {
  beforeEach(() => {
    jest.mocked(listMonthlyFees).mockResolvedValue(buildMonthlyFees([clientFee]));
  });

  it("Then a warning says the extra is not carried over", async () => {
    await renderWithProviders(
      <RecordPaymentScreen clientId={clientFee.clientId} month={feeMonth} />,
    );
    await fireEvent.changeText(
      await screen.findByLabelText(translate("fees.payment.amount")),
      "10000",
    );

    expect(
      await screen.findByText(
        translate("fees.payment.overpaid", { extra: formatMoney(3000, currencyCode) }),
      ),
    ).toBeOnTheScreen();
  });
});
