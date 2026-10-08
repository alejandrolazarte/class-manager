import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { deleteDefaultMonthlyFeeChange, setDefaultMonthlyFee } from "@/features/fees/feesApi";
import { addMonths, formatMonth, monthOf } from "@/features/fees/months";
import { DefaultMonthlyFeeScreen } from "@/features/fees/screens/DefaultMonthlyFeeScreen";
import { translate } from "@/i18n/translate";
import { buildBusiness } from "@/testing/businessFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/fees/feesApi");

const upcomingMonth = addMonths(monthOf(), 1);

describe("When a deleted upcoming default fee change is undone", () => {
  beforeEach(() => {
    jest.mocked(deleteDefaultMonthlyFeeChange).mockResolvedValue(buildBusiness());
    jest.mocked(setDefaultMonthlyFee).mockResolvedValue(buildBusiness());
  });

  it("Then it is scheduled again", async () => {
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

    await fireEvent.press(await screen.findByRole("button", { name: translate("common.undo") }));

    await waitFor(() => expect(setDefaultMonthlyFee).toHaveBeenCalledWith(15000, upcomingMonth));
  });
});
