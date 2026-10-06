import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { deleteDefaultMonthlyFeeChange } from "@/features/fees/feesApi";
import { addMonths, formatMonth, monthOf } from "@/features/fees/months";
import { DefaultMonthlyFeeScreen } from "@/features/fees/screens/DefaultMonthlyFeeScreen";
import { translate } from "@/i18n/translate";
import { buildBusiness } from "@/testing/businessFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/fees/feesApi");

const upcomingMonth = addMonths(monthOf(), 1);

describe("When upcoming default fee change is deleted", () => {
  beforeEach(() => {
    jest.mocked(deleteDefaultMonthlyFeeChange).mockResolvedValue(buildBusiness());
  });

  it("Then it is deleted after confirming", async () => {
    await renderWithProviders(<DefaultMonthlyFeeScreen />, {
      business: buildBusiness({
        defaultMonthlyFeeChanges: [
          { effectiveFrom: "2026-01", amount: 12000 },
          { effectiveFrom: upcomingMonth, amount: 15000 },
        ],
      }),
    });

    await fireEvent.press(
      screen.getByRole("button", {
        name: translate("fees.defaultFee.deleteChange", { month: formatMonth(upcomingMonth) }),
      }),
    );
    expect(deleteDefaultMonthlyFeeChange).not.toHaveBeenCalled();
    await fireEvent.press(screen.getByRole("button", { name: translate("common.confirmDelete") }));

    await waitFor(() => expect(deleteDefaultMonthlyFeeChange).toHaveBeenCalledWith(upcomingMonth));
  });
});
