import { screen } from "@testing-library/react-native";
import { listMonthlyFees } from "@/features/fees/feesApi";
import { MonthlyFeesScreen } from "@/features/fees/screens/MonthlyFeesScreen";
import { translate } from "@/i18n/translate";
import { buildBusiness } from "@/testing/businessFactory";
import {
  buildClassPackClient,
  buildClientFee,
  buildMonthlyFees,
  feeMonth,
} from "@/testing/feeFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/fees/feesApi");

const clientOwingClasses = buildClassPackClient({ unpaidClasses: 2 });
const clientWithOwnFee = buildClientFee({ status: "Unpaid", paid: 0, balance: 12000 });

describe("When business charges with class packs", () => {
  beforeEach(() => {
    jest
      .mocked(listMonthlyFees)
      .mockResolvedValue(buildMonthlyFees([clientWithOwnFee], [clientOwingClasses], "ClassPacks"));
  });

  it("Then clients paying a fee are listed apart", async () => {
    await renderWithProviders(<MonthlyFeesScreen initialMonth={feeMonth} />, {
      business: buildBusiness({ billingMode: "ClassPacks" }),
    });

    expect(
      await screen.findByText(translate("fees.month.monthlyFeeClientsTitle")),
    ).toBeOnTheScreen();
    expect(screen.getByText(clientOwingClasses.clientFullName)).toBeOnTheScreen();
    expect(screen.getByText(clientWithOwnFee.clientFullName)).toBeOnTheScreen();
  });
});
