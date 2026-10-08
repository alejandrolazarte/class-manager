import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import {
  getClassBalance,
  listClassPacks,
  sellClassPack,
} from "@/features/classPacks/classPacksApi";
import { SellClassPackScreen } from "@/features/classPacks/screens/SellClassPackScreen";
import { formatMoney } from "@/features/fees/money";
import { translate } from "@/i18n/translate";
import { buildBusiness } from "@/testing/businessFactory";
import { buildClassBalance, buildClassPack } from "@/testing/classPackFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/classPacks/classPacksApi");

const clientId = "0192f0c4-0000-7000-8000-000000000001";
const classPack = buildClassPack({ name: "ADG De Autor", price: 450 });
const trial = {
  privateLessonId: "0192f0f1-0000-7000-8000-000000000001",
  date: "2026-09-20",
  studentFullName: "Carla Gómez",
  trialPrice: 25,
};

describe("When selling a pack after a paid trial", () => {
  beforeEach(() => {
    jest.mocked(listClassPacks).mockResolvedValue([classPack]);
    jest
      .mocked(getClassBalance)
      .mockResolvedValue(buildClassBalance({ deductibleTrials: [trial] }));
  });

  it("Then the trial is deducted from the price", async () => {
    await renderWithProviders(<SellClassPackScreen clientId={clientId} />);
    await fireEvent.press(
      await screen.findByRole("button", { name: translate("classPacks.sell.choosePack") }),
    );
    await fireEvent.press(await screen.findByRole("button", { name: classPack.name }));

    await fireEvent.press(
      await screen.findByRole("switch", {
        name: translate("classPacks.sell.deductTrial", {
          date: "20/09/2026",
          student: trial.studentFullName,
          amount: formatMoney(trial.trialPrice, buildBusiness().currencyCode),
        }),
      }),
    );
    await fireEvent.press(
      screen.getByRole("button", { name: translate("classPacks.sell.submit") }),
    );

    await waitFor(() =>
      expect(sellClassPack).toHaveBeenCalledWith(
        clientId,
        expect.objectContaining({ price: 425, trialLessonId: trial.privateLessonId }),
      ),
    );
  });
});
