import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { listClassPacks, updateClassPack } from "@/features/classPacks/classPacksApi";
import { ClassPackFormScreen } from "@/features/classPacks/screens/ClassPackFormScreen";
import { addMonths, monthOf } from "@/features/fees/months";
import { translate } from "@/i18n/translate";
import { buildMonthlyPlan } from "@/testing/monthlyPlanFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/classPacks/classPacksApi");
jest.mock("@/features/classGroups/classGroupsApi");

const plan = buildMonthlyPlan();

describe("When a plan price is changed from next month", () => {
  beforeEach(() => {
    jest.mocked(listClassPacks).mockResolvedValue([plan]);
    jest.mocked(updateClassPack).mockResolvedValue(plan);
  });

  it("Then the month is sent", async () => {
    await renderWithProviders(<ClassPackFormScreen classPackId={plan.id} />);

    await fireEvent.changeText(
      await screen.findByLabelText(translate("classPacks.form.price")),
      "28000",
    );
    await fireEvent.press(screen.getByLabelText(translate("fees.effectiveMonth.next")));
    await fireEvent.press(screen.getByRole("button", { name: translate("common.save") }));

    await waitFor(() =>
      expect(updateClassPack).toHaveBeenCalledWith(
        plan.id,
        expect.objectContaining({ price: 28000, priceEffectiveFrom: addMonths(monthOf(), 1) }),
      ),
    );
  });
});
