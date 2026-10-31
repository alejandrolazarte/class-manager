import { screen } from "@testing-library/react-native";
import { listMonthlyFees } from "@/features/fees/feesApi";
import { MonthlyFeesScreen } from "@/features/fees/screens/MonthlyFeesScreen";
import { translate } from "@/i18n/translate";
import {
  buildClassPackClient,
  buildClientFee,
  buildMonthlyFees,
  feeMonth,
} from "@/testing/feeFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/fees/feesApi");
jest.mock("@/features/classPacks/classPacksApi");

const firstClientOwingClasses = buildClassPackClient({ unpaidClasses: 2 });
const secondClientOwingClasses = buildClassPackClient({
  clientId: "0192f0c4-0000-7000-8000-0000000000c2",
  clientFullName: "Paula Suárez",
  unpaidClasses: 1,
});
const clientWithPlan = buildClientFee({ status: "Unpaid", paid: 0, balance: 12000 });

describe("When most families pay per class", () => {
  beforeEach(() => {
    jest
      .mocked(listMonthlyFees)
      .mockResolvedValue(
        buildMonthlyFees([clientWithPlan], [firstClientOwingClasses, secondClientOwingClasses]),
      );
  });

  it("Then families with a plan are listed apart", async () => {
    await renderWithProviders(<MonthlyFeesScreen initialMonth={feeMonth} />);

    expect(
      await screen.findByText(translate("fees.month.monthlyFeeClientsTitle")),
    ).toBeOnTheScreen();
    expect(screen.getByText(clientWithPlan.clientFullName)).toBeOnTheScreen();
  });
});
