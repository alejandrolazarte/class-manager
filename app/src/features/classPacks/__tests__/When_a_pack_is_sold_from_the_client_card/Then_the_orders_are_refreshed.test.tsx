import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { listClassPacks, sellClassPack } from "@/features/classPacks/classPacksApi";
import { SellClassPackScreen } from "@/features/classPacks/screens/SellClassPackScreen";
import { listOrders } from "@/features/orders/ordersApi";
import { useOrders } from "@/features/orders/useOrders";
import { translate } from "@/i18n/translate";
import { buildClassPack } from "@/testing/classPackFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/classPacks/classPacksApi");
jest.mock("@/features/orders/ordersApi");

const clientId = "0192f0c4-0000-7000-8000-000000000001";
const classPack = buildClassPack();

function OrdersProbe() {
  useOrders(null, "all");
  return null;
}

describe("When a pack is sold from the client card", () => {
  beforeEach(() => {
    jest.mocked(listOrders).mockResolvedValue([]);
    jest.mocked(listClassPacks).mockResolvedValue([classPack]);
    jest.mocked(sellClassPack).mockResolvedValue({
      id: "purchase",
      clientId,
      name: classPack.name,
      classCount: classPack.classCount,
      price: classPack.price,
      purchasedOn: "2026-09-05",
      expiresOn: null,
      method: "Cash",
      notes: null,
      classDurationMinutes: null,
      trialLessonId: null,
    });
  });

  it("Then the orders are refreshed", async () => {
    await renderWithProviders(
      <>
        <OrdersProbe />
        <SellClassPackScreen clientId={clientId} />
      </>,
    );
    await waitFor(() => expect(listOrders).toHaveBeenCalledTimes(1));

    await fireEvent.press(await screen.findByRole("button", { name: classPack.name }));
    await fireEvent.press(
      screen.getByRole("button", { name: translate("classPacks.sell.submit") }),
    );

    await waitFor(() => expect(listOrders).toHaveBeenCalledTimes(2));
  });
});
