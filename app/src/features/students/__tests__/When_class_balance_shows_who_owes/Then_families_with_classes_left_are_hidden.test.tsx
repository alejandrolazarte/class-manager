import { fireEvent, screen } from "@testing-library/react-native";
import { listMonthlyFees } from "@/features/fees/feesApi";
import { StudentListScreen } from "@/features/students/screens/StudentListScreen";
import { searchStudents } from "@/features/students/studentsApi";
import { translate } from "@/i18n/translate";
import { buildClassPackClient, buildMonthlyFees } from "@/testing/feeFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/fees/feesApi");
jest.mock("@/features/students/studentsApi");

const clientOwingClasses = buildClassPackClient({
  clientId: "owing",
  clientFullName: "Zoe Ruiz",
  availableClasses: 0,
  unpaidClasses: 2,
});
const clientWithClassesLeft = buildClassPackClient({
  clientId: "left",
  clientFullName: "Aldana Paz",
  availableClasses: 3,
  unpaidClasses: 0,
});

describe("When class balance shows who owes", () => {
  beforeEach(() => {
    jest.mocked(searchStudents).mockResolvedValue([]);
    jest
      .mocked(listMonthlyFees)
      .mockResolvedValue(buildMonthlyFees([], [clientOwingClasses, clientWithClassesLeft]));
  });

  it("Then families with classes left are hidden", async () => {
    await renderWithProviders(<StudentListScreen />);
    await fireEvent.press(
      await screen.findByRole("tab", { name: translate("classBalances.title") }),
    );
    await screen.findByText(clientWithClassesLeft.clientFullName);

    await fireEvent.press(
      screen.getByRole("button", { name: translate("classBalances.filter.owingLabel") }),
    );

    expect(screen.queryByText(clientWithClassesLeft.clientFullName)).not.toBeOnTheScreen();
    expect(screen.getByText(clientOwingClasses.clientFullName)).toBeOnTheScreen();
  });
});
