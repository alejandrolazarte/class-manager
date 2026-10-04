import { fireEvent, screen, waitFor } from "@testing-library/react-native";
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

describe("When student cancels an unpaid order", () => {
  beforeEach(() => {
    jest.mocked(getStudentAppHome).mockResolvedValue(buildStudentAppHome());
    jest.mocked(listStudentAppOrders).mockResolvedValue([buildStudentAppOrder()]);
    jest
      .mocked(cancelStudentAppOrder)
      .mockResolvedValue(buildStudentAppOrder({ status: "Cancelled" }));
  });

  it("Then the cancellation is sent", async () => {
    await renderStudentAppScreen(<StudentAppOrdersScreen />);

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("student.orders.cancel") }),
    );

    await waitFor(() => expect(cancelStudentAppOrder).toHaveBeenCalledWith("order-1"));
  });
});
