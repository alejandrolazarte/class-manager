import { screen } from "@testing-library/react-native";
import { getStudentAppHome, listStudentAppOrders } from "@/features/studentApp/studentAppApi";
import { StudentAppOrdersScreen } from "@/features/studentApp/screens/StudentAppOrdersScreen";
import { buildStudentAppHome, buildStudentAppOrder } from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");

describe("When student opens its orders", () => {
  beforeEach(() => {
    jest.mocked(getStudentAppHome).mockResolvedValue(buildStudentAppHome());
    jest.mocked(listStudentAppOrders).mockResolvedValue([buildStudentAppOrder({ number: 57 })]);
  });

  it("Then each order shows its number", async () => {
    await renderStudentAppScreen(<StudentAppOrdersScreen />);

    expect(await screen.findByText(/Pedido n\.º 57 del/)).toBeOnTheScreen();
  });
});
