import { screen } from "@testing-library/react-native";
import { listMonthlyFees } from "@/features/fees/feesApi";
import { MonthlyFeesScreen } from "@/features/fees/screens/MonthlyFeesScreen";
import { translate } from "@/i18n/translate";
import { buildClientFee, buildMonthlyFees, feeMonth } from "@/testing/feeFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/fees/feesApi");

const clientWithoutFee = buildClientFee({ fee: null, paid: 0, balance: 0, status: "NoFee" });

describe("When month has no fee", () => {
  beforeEach(() => {
    jest.mocked(listMonthlyFees).mockResolvedValue(buildMonthlyFees([clientWithoutFee]));
  });

  it("Then it does not say everybody paid", async () => {
    await renderWithProviders(<MonthlyFeesScreen initialMonth={feeMonth} />);

    expect(await screen.findByText(translate("fees.month.noFeeDue"))).toBeOnTheScreen();
  });
});
