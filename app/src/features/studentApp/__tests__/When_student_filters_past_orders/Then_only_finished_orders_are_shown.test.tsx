import { fireEvent, screen } from "@testing-library/react-native";
import { getStudentAppHome, listStudentAppOrders } from "@/features/studentApp/studentAppApi";
import { StudentAppOrdersScreen } from "@/features/studentApp/screens/StudentAppOrdersScreen";
import { translate } from "@/i18n/translate";
import { buildStudentAppHome, buildStudentAppOrder } from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");

describe("When student filters past orders", () => {
  beforeEach(() => {
    jest.mocked(getStudentAppHome).mockResolvedValue(buildStudentAppHome());
    jest
      .mocked(listStudentAppOrders)
      .mockResolvedValue([
        buildStudentAppOrder({ id: "order-open" }),
        buildStudentAppOrder({ id: "order-delivered", status: "Delivered" }),
      ]);
  });

  it("Then only finished orders are shown", async () => {
    await renderStudentAppScreen(<StudentAppOrdersScreen />);

    expect(await screen.findByText(translate("student.orders.status.Requested"))).toBeOnTheScreen();

    await fireEvent.press(
      screen.getByRole("button", { name: new RegExp(translate("student.orders.filter.past")) }),
    );

    expect(screen.getByText(translate("student.orders.status.Delivered"))).toBeOnTheScreen();
    expect(screen.queryByText(translate("student.orders.status.Requested"))).toBeNull();
  });
});
