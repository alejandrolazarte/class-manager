import { screen } from "@testing-library/react-native";
import { getStudentAppHome, listStudentAppOrders } from "@/features/studentApp/studentAppApi";
import { StudentAppOrdersScreen } from "@/features/studentApp/screens/StudentAppOrdersScreen";
import { translate } from "@/i18n/translate";
import { buildStudentAppHome, buildStudentAppOrder } from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");

describe("When an order is ready in class", () => {
  beforeEach(() => {
    jest.mocked(getStudentAppHome).mockResolvedValue(buildStudentAppHome());
    jest.mocked(listStudentAppOrders).mockResolvedValue([
      buildStudentAppOrder({
        status: "Paid",
        awaitsPickup: true,
        isReady: true,
        delivery: "InClass",
        deliveryClassGroupName: "Natación inicial",
        lines: [
          {
            id: "line-cap",
            kind: "Product",
            name: "Gorro de natación · S",
            quantity: 1,
            total: 12,
            refundedQuantity: 0,
          },
        ],
      }),
    ]);
  });

  it("Then the class is shown", async () => {
    await renderStudentAppScreen(<StudentAppOrdersScreen />);

    expect(await screen.findByText(translate("student.orders.status.ready"))).toBeOnTheScreen();
    expect(
      screen.getByText(
        `${translate("student.orders.deliveryInClass", { className: "Natación inicial" })}.`,
      ),
    ).toBeOnTheScreen();
  });
});
