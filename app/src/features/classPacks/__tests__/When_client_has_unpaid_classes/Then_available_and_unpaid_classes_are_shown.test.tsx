import { screen } from "@testing-library/react-native";
import { getClassBalance } from "@/features/classPacks/classPacksApi";
import { ClassBalanceSection } from "@/features/classPacks/components/ClassBalanceSection";
import { translateCount } from "@/i18n/translate";
import { buildClassBalance } from "@/testing/classPackFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/classPacks/classPacksApi");

const clientId = "0192f0c4-0000-7000-8000-000000000001";

describe("When client has unpaid classes", () => {
  beforeEach(() => {
    jest.mocked(getClassBalance).mockResolvedValue(
      buildClassBalance({
        availableClasses: 0,
        unpaidClasses: 2,
        unpaidAttendances: [
          {
            date: "2026-09-12",
            studentFullName: "Mateo Díaz",
            classGroupName: "Natación",
            isPrivateLesson: false,
          },
          {
            date: "2026-09-19",
            studentFullName: "Mateo Díaz",
            classGroupName: "Natación",
            isPrivateLesson: false,
          },
        ],
      }),
    );
  });

  it("Then available and unpaid classes are shown", async () => {
    await renderWithProviders(<ClassBalanceSection clientId={clientId} />);

    expect(
      await screen.findByText(translateCount("classPacks.balance.unpaid", 2)),
    ).toBeOnTheScreen();
    expect(screen.getByText(translateCount("classPacks.balance.available", 0))).toBeOnTheScreen();
  });
});
