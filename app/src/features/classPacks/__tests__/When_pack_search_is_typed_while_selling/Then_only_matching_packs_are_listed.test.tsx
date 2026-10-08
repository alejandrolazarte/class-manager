import { fireEvent, screen, within } from "@testing-library/react-native";
import { getClassBalance, listClassPacks } from "@/features/classPacks/classPacksApi";
import { SellClassPackScreen } from "@/features/classPacks/screens/SellClassPackScreen";
import { translate } from "@/i18n/translate";
import { buildClassBalance, buildClassPack } from "@/testing/classPackFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/classPacks/classPacksApi");

const eightClasses = buildClassPack({ id: "class-pack-eight", name: "8 clases" });
const monthly = buildClassPack({ id: "class-pack-monthly", name: "Mensual libre" });

describe("When pack search is typed while selling", () => {
  beforeEach(() => {
    jest.mocked(listClassPacks).mockResolvedValue([eightClasses, monthly]);
    jest.mocked(getClassBalance).mockResolvedValue(buildClassBalance({ purchases: [] }));
  });

  it("Then only matching packs are listed", async () => {
    await renderWithProviders(<SellClassPackScreen clientId="client-1" />);
    await fireEvent.press(
      await screen.findByRole("button", { name: translate("classPacks.sell.choosePack") }),
    );
    const picker = within(screen.getByTestId("sell-class-pack-picker"));

    await fireEvent.changeText(
      picker.getByPlaceholderText(translate("classPacks.picker.search")),
      "mensual",
    );

    expect(await picker.findByRole("button", { name: monthly.name })).toBeOnTheScreen();
    expect(picker.queryByRole("button", { name: eightClasses.name })).toBeNull();
  });
});
