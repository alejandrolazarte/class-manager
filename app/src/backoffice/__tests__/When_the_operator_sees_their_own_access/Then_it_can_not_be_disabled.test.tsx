import { screen } from "@testing-library/react-native";
import { getOperators } from "@/backoffice/operators/operatorsApi";
import { OperatorsScreen } from "@/backoffice/operators/OperatorsScreen";
import { translate } from "@/i18n/translate";
import { anOperatorList } from "@/testing/backofficeOperators";
import { renderBackofficeScreen } from "@/testing/renderBackofficeScreen";

jest.mock("@/backoffice/operators/operatorsApi", () => ({
  ...jest.requireActual("@/backoffice/operators/operatorsApi"),
  getOperators: jest.fn(),
}));

describe("When the operator sees their own access", () => {
  it("Then it can not be disabled", async () => {
    jest.mocked(getOperators).mockResolvedValue(anOperatorList());

    await renderBackofficeScreen(<OperatorsScreen />);

    expect(await screen.findByText(translate("backoffice.operators.you"))).toBeOnTheScreen();
    expect(
      screen.queryByRole("button", {
        name: translate("backoffice.operators.disableOf", { name: "Operadora Uno" }),
      }),
    ).toBeNull();
  });
});
