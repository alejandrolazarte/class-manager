import { screen } from "@testing-library/react-native";

import { getStudentAppHome, listStudentAppOrders } from "@/features/studentApp/studentAppApi";
import { StudentAppOrdersScreen } from "@/features/studentApp/screens/StudentAppOrdersScreen";
import { formatMoney } from "@/formatting/money";
import { translate } from "@/i18n/translate";

import { buildStudentAppHome, buildStudentAppOrder } from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");
jest.mock("@/features/studentApp/orderCheckoutRedirect");

const payableOrder = buildStudentAppOrder({ canPayOnline: true });
const payNowLabel = translate("student.orders.payNow", {
  amount: formatMoney(payableOrder.balance, payableOrder.currencyCode),
});

describe("When an order cannot be paid online", () => {
  beforeEach(() => {
    jest.mocked(getStudentAppHome).mockResolvedValue(buildStudentAppHome());
    jest.mocked(listStudentAppOrders).mockResolvedValue([buildStudentAppOrder()]);
  });

  it("Then there is no pay button", async () => {
    await renderStudentAppScreen(<StudentAppOrdersScreen />);

    await screen.findByText(translate("student.orders.title"));

    expect(screen.queryByText(payNowLabel)).toBeNull();
  });
});
