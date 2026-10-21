import { screen } from "@testing-library/react-native";
import { getOrder } from "@/features/orders/ordersApi";
import { OrderDetailScreen } from "@/features/orders/screens/OrderDetailScreen";
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

const openedOrder = { ...paidOrder, payments: [otherPayment] };

describe("When member opens payments someone else recorded", () => {
  beforeEach(() => {
    jest.mocked(getOrder).mockResolvedValue(openedOrder);
  });

  it("Then they cannot delete them", async () => {
    await renderWithProviders(<OrderDetailScreen orderId={openedOrder.id} />, { member });

    expect(
      await screen.findByText(translate("fees.methods.Cash"), { exact: false }),
    ).toBeOnTheScreen();
    expect(
      screen.queryByRole("button", {
        name: translate("orders.payments.deleteOf", {
          amount: formatMoney(otherPayment.amount, "ARS"),
        }),
      }),
    ).toBeNull();
  });
});
