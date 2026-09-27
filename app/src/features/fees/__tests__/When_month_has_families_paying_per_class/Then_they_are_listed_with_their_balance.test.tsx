import { screen } from "@testing-library/react-native";
import { listMonthlyFees } from "@/features/fees/feesApi";
import { MonthlyFeesScreen } from "@/features/fees/screens/MonthlyFeesScreen";
import { translate, translateCount } from "@/i18n/translate";
import { buildClassPackClient, buildMonthlyFees, feeMonth } from "@/testing/feeFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/fees/feesApi");

const familyOwingClasses = buildClassPackClient({ unpaidClasses: 2 });

describe("When month has families paying per class", () => {
  beforeEach(() => {
    jest.mocked(listMonthlyFees).mockResolvedValue(buildMonthlyFees([], [familyOwingClasses]));
  });

  it("Then they are listed with their balance", async () => {
    await renderWithProviders(<MonthlyFeesScreen initialMonth={feeMonth} />);

    expect(await screen.findByText(familyOwingClasses.clientFullName)).toBeOnTheScreen();
    expect(screen.getByText(translate("fees.month.classPacksTitle"))).toBeOnTheScreen();
    expect(screen.getByText(translateCount("classPacks.balance.unpaid", 2))).toBeOnTheScreen();
  });
});
