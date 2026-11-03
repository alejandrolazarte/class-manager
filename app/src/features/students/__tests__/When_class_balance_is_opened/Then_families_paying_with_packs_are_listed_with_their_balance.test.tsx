import { fireEvent, screen } from "@testing-library/react-native";
import { listMonthlyFees } from "@/features/fees/feesApi";
import { StudentListScreen } from "@/features/students/screens/StudentListScreen";
import { searchStudents } from "@/features/students/studentsApi";
import { translate, translateCount } from "@/i18n/translate";
import { buildClassPackClient, buildMonthlyFees } from "@/testing/feeFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/fees/feesApi");
jest.mock("@/features/students/studentsApi");

const clientOwingClasses = buildClassPackClient({ availableClasses: 0, unpaidClasses: 2 });

describe("When class balance is opened", () => {
  beforeEach(() => {
    jest.mocked(searchStudents).mockResolvedValue([]);
    jest.mocked(listMonthlyFees).mockResolvedValue(buildMonthlyFees([], [clientOwingClasses]));
  });

  it("Then families paying with packs are listed with their balance", async () => {
    await renderWithProviders(<StudentListScreen />);

    await fireEvent.press(
      await screen.findByRole("tab", { name: translate("classBalances.title") }),
    );

    expect(await screen.findByText(clientOwingClasses.clientFullName)).toBeOnTheScreen();
    expect(screen.getByText(translateCount("classPacks.balance.unpaid", 2))).toBeOnTheScreen();
  });
});
