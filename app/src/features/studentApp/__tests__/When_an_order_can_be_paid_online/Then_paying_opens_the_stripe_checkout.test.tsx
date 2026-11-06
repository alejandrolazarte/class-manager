import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { openOrderCheckout } from "@/features/studentApp/orderCheckoutRedirect";
import {
  getStudentAppHome,
  listStudentAppOrders,
  startStudentAppOrderCheckout,
} from "@/features/studentApp/studentAppApi";
import { StudentAppOrdersScreen } from "@/features/studentApp/screens/StudentAppOrdersScreen";
import { formatMoney } from "@/formatting/money";
import { translate } from "@/i18n/translate";

import { buildStudentAppHome, buildStudentAppOrder } from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");
jest.mock("@/features/studentApp/orderCheckoutRedirect");

const checkoutUrl = "https://checkout.stripe.test/order-1";
const payableOrder = buildStudentAppOrder({ canPayOnline: true });
const payNowLabel = translate("student.orders.payNow", {
  amount: formatMoney(payableOrder.balance, payableOrder.currencyCode),
});

describe("When an order can be paid online", () => {
  beforeEach(() => {
    jest.mocked(getStudentAppHome).mockResolvedValue(buildStudentAppHome());
    jest.mocked(listStudentAppOrders).mockResolvedValue([payableOrder]);
    jest.mocked(startStudentAppOrderCheckout).mockResolvedValue({ url: checkoutUrl });
  });

  it("Then paying opens the stripe checkout", async () => {
    await renderStudentAppScreen(<StudentAppOrdersScreen />);

    fireEvent.press(await screen.findByText(payNowLabel));

    await waitFor(() => expect(openOrderCheckout).toHaveBeenCalledWith(checkoutUrl));
  });
});
