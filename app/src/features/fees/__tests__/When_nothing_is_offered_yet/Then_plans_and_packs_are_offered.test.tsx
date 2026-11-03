import { screen } from "@testing-library/react-native";
import { listClassPacks } from "@/features/classPacks/classPacksApi";
import { listMonthlyFees } from "@/features/fees/feesApi";
import { CollectionsScreen } from "@/features/fees/screens/CollectionsScreen";
import { translate } from "@/i18n/translate";
import { buildMonthlyFees, feeMonth } from "@/testing/feeFactory";
import { buildOrderSummary } from "@/testing/productFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { getOrderSummary, listOrders } from "@/features/orders/ordersApi";

jest.mock("@/features/fees/feesApi");
jest.mock("@/features/orders/ordersApi");
jest.mock("@/features/classPacks/classPacksApi");

describe("When nothing is offered yet", () => {
  beforeEach(() => {
    jest.mocked(getOrderSummary).mockResolvedValue(buildOrderSummary());
    jest.mocked(listOrders).mockResolvedValue([]);
  });

  beforeEach(() => {
    jest.mocked(listMonthlyFees).mockResolvedValue(buildMonthlyFees([]));
    jest.mocked(listClassPacks).mockResolvedValue([]);
  });

  it("Then plans and packs are offered", async () => {
    await renderWithProviders(<CollectionsScreen initialMonth={feeMonth} />);

    expect(
      await screen.findByRole("button", { name: translate("fees.month.goToPlans") }),
    ).toBeOnTheScreen();
  });
});
