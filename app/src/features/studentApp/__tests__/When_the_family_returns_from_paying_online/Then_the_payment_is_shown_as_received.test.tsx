import { screen } from "@testing-library/react-native";

import { getStudentAppHome, listStudentAppOrders } from "@/features/studentApp/studentAppApi";
import { StudentAppOrdersScreen } from "@/features/studentApp/screens/StudentAppOrdersScreen";

import { translate } from "@/i18n/translate";
import { searchParametersMock } from "@/testing/expoRouterMock";
import { buildStudentAppHome, buildStudentAppOrder } from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");
jest.mock("@/features/studentApp/orderCheckoutRedirect");

describe("When the family returns from paying online", () => {
  beforeEach(() => {
    jest.mocked(getStudentAppHome).mockResolvedValue(buildStudentAppHome());
    searchParametersMock.current = { payment: "success" };
    jest.mocked(listStudentAppOrders).mockResolvedValue([buildStudentAppOrder()]);
  });

  it("Then the payment is shown as received", async () => {
    await renderStudentAppScreen(<StudentAppOrdersScreen />);

    expect(await screen.findByText(translate("student.orders.paymentReceived"))).toBeOnTheScreen();
  });
});
