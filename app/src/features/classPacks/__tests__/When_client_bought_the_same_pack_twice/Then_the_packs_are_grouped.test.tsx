import { fireEvent, screen } from "@testing-library/react-native";
import { getClassBalance } from "@/features/classPacks/classPacksApi";
import { ClassBalanceSection } from "@/features/classPacks/components/ClassBalanceSection";
import { translate } from "@/i18n/translate";
import { buildClassBalance } from "@/testing/classPackFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/classPacks/classPacksApi");

const clientId = "0192f0c4-0000-7000-8000-000000000001";
const balance = buildClassBalance();
const purchase = balance.purchases[0];
const groupName = translate("classPacks.balance.groupTitle", { name: purchase.name, count: 2 });

describe("When client bought the same pack twice", () => {
  beforeEach(() => {
    jest.mocked(getClassBalance).mockResolvedValue({
      ...balance,
      purchases: [purchase, { ...purchase, id: "0192f0c4-0000-7000-8000-0000000b0a12" }],
    });
  });

  it("Then the packs are grouped", async () => {
    await renderWithProviders(<ClassBalanceSection clientId={clientId} />);

    await fireEvent.press(
      await screen.findByRole("button", {
        name: translate("classPacks.balance.showPacks", { name: groupName }),
      }),
    );

    expect(screen.getByText(groupName)).toBeOnTheScreen();
    expect(screen.getAllByText(/^Pack \d/)).toHaveLength(2);
  });
});
