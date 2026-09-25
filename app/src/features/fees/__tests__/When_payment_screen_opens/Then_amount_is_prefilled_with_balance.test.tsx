import { screen } from "@testing-library/react-native";
import { listMonthlyFees } from "@/features/fees/feesApi";
import { RecordPaymentScreen } from "@/features/fees/screens/RecordPaymentScreen";
import { translate } from "@/i18n/translate";
import { buildClientFee, buildMonthlyFees, feeMonth } from "@/testing/feeFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/fees/feesApi");

const clientFee = buildClientFee();

describe("When payment screen opens", () => {
  beforeEach(() => {
    jest.mocked(listMonthlyFees).mockResolvedValue(buildMonthlyFees([clientFee]));
  });

  it("Then amount is prefilled with balance", async () => {
    await renderWithProviders(
      <RecordPaymentScreen clientId={clientFee.clientId} month={feeMonth} />,
    );

    expect(await screen.findByLabelText(translate("fees.payment.amount"))).toHaveDisplayValue(
      "7000",
    );
  });
});
