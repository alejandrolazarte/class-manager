import { screen } from "@testing-library/react-native";
import { listMonthlyFees } from "@/features/fees/feesApi";
import { CollectionsScreen } from "@/features/fees/screens/CollectionsScreen";
import { formatMonth } from "@/features/fees/months";
import { translate } from "@/i18n/translate";
import { buildMonthlyFees, feeMonth } from "@/testing/feeFactory";
import { buildCurrentMember } from "@/testing/memberFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/fees/feesApi");

describe("When member cannot see orders", () => {
  beforeEach(() => {
    jest.mocked(listMonthlyFees).mockResolvedValue(buildMonthlyFees([]));
  });

  it("Then only fees are shown", async () => {
    await renderWithProviders(<CollectionsScreen initialMonth={feeMonth} />, {
      member: buildCurrentMember({ permissions: ["business.view", "payments.view.all"] }),
    });

    expect(await screen.findByText(formatMonth(feeMonth))).toBeOnTheScreen();
    expect(screen.queryByRole("button", { name: translate("collections.orders") })).toBeNull();
  });
});
