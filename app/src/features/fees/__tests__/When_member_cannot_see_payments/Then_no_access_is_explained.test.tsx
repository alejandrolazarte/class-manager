import { screen } from "@testing-library/react-native";
import { listMonthlyFees } from "@/features/fees/feesApi";
import { CollectionsScreen } from "@/features/fees/screens/CollectionsScreen";
import { translate } from "@/i18n/translate";
import { feeMonth } from "@/testing/feeFactory";
import { buildCurrentMember } from "@/testing/memberFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/fees/feesApi");

describe("When member cannot see payments", () => {
  it("Then no access is explained", async () => {
    await renderWithProviders(<CollectionsScreen initialMonth={feeMonth} />, {
      member: buildCurrentMember({ permissions: ["business.view"] }),
    });

    expect(await screen.findByText(translate("fees.noAccess"))).toBeOnTheScreen();
    expect(listMonthlyFees).not.toHaveBeenCalled();
  });
});
