import { screen } from "@testing-library/react-native";
import { listMonthlyFees } from "@/features/fees/feesApi";
import { MonthlyFeesScreen } from "@/features/fees/screens/MonthlyFeesScreen";
import { translate, translateCount } from "@/i18n/translate";
import { buildClassPackClient, buildMonthlyFees, feeMonth } from "@/testing/feeFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/fees/feesApi");

const clientOwingClasses = buildClassPackClient({ unpaidClasses: 2 });

describe("When month has clients paying per class", () => {
  beforeEach(() => {
    jest.mocked(listMonthlyFees).mockResolvedValue(buildMonthlyFees([], [clientOwingClasses]));
  });

  it("Then they are listed with their balance", async () => {
    await renderWithProviders(<MonthlyFeesScreen initialMonth={feeMonth} />);

    expect(await screen.findByText(clientOwingClasses.clientFullName)).toBeOnTheScreen();
    expect(screen.getByText(translate("fees.month.classPacksTitle"))).toBeOnTheScreen();
    expect(screen.getByText(translateCount("classPacks.balance.unpaid", 2))).toBeOnTheScreen();
  });
});
