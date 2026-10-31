import { fireEvent, screen } from "@testing-library/react-native";
import { listClassPacks } from "@/features/classPacks/classPacksApi";
import { SellClassPackScreen } from "@/features/classPacks/screens/SellClassPackScreen";
import { translate } from "@/i18n/translate";
import { buildClassPack } from "@/testing/classPackFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/classPacks/classPacksApi");

const clientId = "0192f0c4-0000-7000-8000-000000000001";
const pack = buildClassPack();
const monthlyPlan = buildClassPack({
  id: "0192f0c4-0000-7000-8000-00000000c1a6",
  name: "Cuota mensual",
  classCount: null,
  validityMonths: null,
  renewsMonthly: true,
  appliesFamilyDiscount: true,
});

describe("When selling a pack and the business has a monthly plan", () => {
  beforeEach(() => {
    jest.mocked(listClassPacks).mockResolvedValue([pack, monthlyPlan]);
  });

  it("Then the plan is not offered", async () => {
    await renderWithProviders(<SellClassPackScreen clientId={clientId} />);

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("classPacks.sell.choosePack") }),
    );
    await screen.findByRole("button", { name: pack.name });

    expect(screen.queryByRole("button", { name: monthlyPlan.name })).toBeNull();
  });
});
