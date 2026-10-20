import { fireEvent, screen } from "@testing-library/react-native";
import {
  cancelStudentAppOrder,
  getStudentAppHome,
  listStudentAppOrders,
} from "@/features/studentApp/studentAppApi";
import { StudentAppOrdersScreen } from "@/features/studentApp/screens/StudentAppOrdersScreen";
import { translate } from "@/i18n/translate";
import { buildStudentAppHome, buildStudentAppOrder } from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");

const order = buildStudentAppOrder();

describe("When student goes back from cancelling an order", () => {
  beforeEach(() => {
    jest.mocked(getStudentAppHome).mockResolvedValue(buildStudentAppHome());
    jest.mocked(listStudentAppOrders).mockResolvedValue([order]);
  });

  it("Then the order is not cancelled", async () => {
    await renderStudentAppScreen(<StudentAppOrdersScreen />);

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("student.orders.cancel") }),
    );
    await fireEvent.press(
      screen.getAllByRole("button", { name: translate("common.back") }).at(-1)!,
    );

    expect(
      screen.queryByText(translate("student.orders.cancelQuestion", { number: order.number })),
    ).not.toBeOnTheScreen();
    expect(cancelStudentAppOrder).not.toHaveBeenCalled();
  });
});
