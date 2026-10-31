import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { deleteClassPackPriceChange, listClassPacks } from "@/features/classPacks/classPacksApi";
import { ClassPackFormScreen } from "@/features/classPacks/screens/ClassPackFormScreen";
import { addMonths, formatMonth, monthOf } from "@/features/fees/months";
import { translate } from "@/i18n/translate";
import { buildMonthlyPlan } from "@/testing/monthlyPlanFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/classPacks/classPacksApi");
jest.mock("@/features/classGroups/classGroupsApi");

const upcomingMonth = addMonths(monthOf(), 1);
const plan = buildMonthlyPlan({ priceChanges: [{ effectiveFrom: upcomingMonth, price: 28000 }] });

describe("When an upcoming plan price is deleted", () => {
  beforeEach(() => {
    jest.mocked(listClassPacks).mockResolvedValue([plan]);
    jest.mocked(deleteClassPackPriceChange).mockResolvedValue(plan);
  });

  it("Then it is deleted at once", async () => {
    await renderWithProviders(<ClassPackFormScreen classPackId={plan.id} />);

    await fireEvent.press(
      await screen.findByRole("button", {
        name: translate("classPacks.form.deletePriceChange", { month: formatMonth(upcomingMonth) }),
      }),
    );

    await waitFor(() =>
      expect(deleteClassPackPriceChange).toHaveBeenCalledWith(plan.id, upcomingMonth),
    );
  });
});
