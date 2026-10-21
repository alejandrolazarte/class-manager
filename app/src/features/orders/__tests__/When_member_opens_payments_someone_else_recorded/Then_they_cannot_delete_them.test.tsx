import { fireEvent, screen } from "@testing-library/react-native";
import { listOrders } from "@/features/orders/ordersApi";
import { OrderListScreen } from "@/features/orders/screens/OrderListScreen";
import { permissions } from "@/features/members/permissions";
import { formatMoney } from "@/formatting/money";
import { translate } from "@/i18n/translate";
import { buildCurrentMember } from "@/testing/memberFactory";
import { buildOrder } from "@/testing/productFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/orders/ordersApi");

const paidOrder = buildOrder();
const otherPayment = { ...paidOrder.payments[0], recordedByUserId: "user-owner" };
const member = buildCurrentMember({
  userId: "user-seller",
  permissions: [permissions.ordersViewOwn, permissions.ordersManage],
});

describe("When member opens payments someone else recorded", () => {
  beforeEach(() => {
    jest.mocked(listOrders).mockResolvedValue([{ ...paidOrder, payments: [otherPayment] }]);
  });

  it("Then they cannot delete them", async () => {
    await renderWithProviders(<OrderListScreen />, { member });

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("orders.payments.open") }),
    );

    expect(screen.getByText(translate("fees.methods.Cash"), { exact: false })).toBeOnTheScreen();
    expect(
      screen.queryByRole("button", {
        name: translate("orders.payments.deleteOf", {
          amount: formatMoney(otherPayment.amount, "ARS"),
        }),
      }),
    ).toBeNull();
  });
});
