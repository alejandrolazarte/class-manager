import { act, screen, waitFor } from "@testing-library/react-native";
import { useFocusEffect } from "expo-router";
import { listMonthlyFees } from "@/features/fees/feesApi";
import { CollectionsScreen } from "@/features/fees/screens/CollectionsScreen";
import { buildClientFee, buildMonthlyFees, feeMonth } from "@/testing/feeFactory";
import { buildOrderSummary } from "@/testing/productFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { getOrderSummary, listOrders } from "@/features/orders/ordersApi";

jest.mock("@/features/fees/feesApi");
jest.mock("@/features/orders/ordersApi");

const enrolledByAnotherInstructor = buildClientFee({
  clientId: "new-client",
  clientFullName: "Zoe Ruiz",
  paid: 0,
  balance: 12000,
  status: "Unpaid",
});

describe("When cobros gets focus again", () => {
  beforeEach(() => {
    jest.mocked(getOrderSummary).mockResolvedValue(buildOrderSummary());
    jest.mocked(listOrders).mockResolvedValue([]);
  });

  it("Then the month is reloaded", async () => {
    const focusCallbacks: (() => void)[] = [];
    jest.mocked(useFocusEffect).mockImplementation((callback) => {
      focusCallbacks.push(callback as () => void);
    });
    jest.mocked(listMonthlyFees).mockResolvedValueOnce(buildMonthlyFees([]));
    await renderWithProviders(<CollectionsScreen initialMonth={feeMonth} />);
    await waitFor(() => expect(listMonthlyFees).toHaveBeenCalledTimes(1));
    focusCallbacks.at(-1)?.();
    jest
      .mocked(listMonthlyFees)
      .mockResolvedValueOnce(buildMonthlyFees([enrolledByAnotherInstructor]));

    await act(() => focusCallbacks.at(-1)?.());

    expect(await screen.findByText(enrolledByAnotherInstructor.clientFullName)).toBeOnTheScreen();
  });
});
