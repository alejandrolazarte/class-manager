import { act, screen, waitFor } from "@testing-library/react-native";
import { useFocusEffect } from "expo-router";
import { listMonthlyFees } from "@/features/fees/feesApi";
import { MonthlyFeesScreen } from "@/features/fees/screens/MonthlyFeesScreen";
import { buildClientFee, buildMonthlyFees, feeMonth } from "@/testing/feeFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/fees/feesApi");

const enrolledByAnotherInstructor = buildClientFee({
  clientId: "new-client",
  clientFullName: "Zoe Ruiz",
  paid: 0,
  balance: 12000,
  status: "Unpaid",
});

describe("When cobros gets focus again", () => {
  it("Then the month is reloaded", async () => {
    const focusCallbacks: (() => void)[] = [];
    jest.mocked(useFocusEffect).mockImplementation((callback) => {
      focusCallbacks.push(callback as () => void);
    });
    jest.mocked(listMonthlyFees).mockResolvedValueOnce(buildMonthlyFees([]));
    await renderWithProviders(<MonthlyFeesScreen initialMonth={feeMonth} />);
    await waitFor(() => expect(listMonthlyFees).toHaveBeenCalledTimes(1));
    focusCallbacks.at(-1)?.();
    jest
      .mocked(listMonthlyFees)
      .mockResolvedValueOnce(buildMonthlyFees([enrolledByAnotherInstructor]));

    await act(() => focusCallbacks.at(-1)?.());

    expect(await screen.findByText(enrolledByAnotherInstructor.clientFullName)).toBeOnTheScreen();
  });
});
