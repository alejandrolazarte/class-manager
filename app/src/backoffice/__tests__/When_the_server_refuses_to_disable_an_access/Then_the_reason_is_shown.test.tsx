import { fireEvent, screen } from "@testing-library/react-native";
import { BackofficeRequestError } from "@/backoffice/api/backofficeHttpClient";
import { disableOperator, getOperators } from "@/backoffice/operators/operatorsApi";
import { OperatorsScreen } from "@/backoffice/operators/OperatorsScreen";
import { translate } from "@/i18n/translate";
import { anOperatorList } from "@/testing/backofficeOperators";
import { renderBackofficeScreen } from "@/testing/renderBackofficeScreen";

jest.mock("@/backoffice/operators/operatorsApi", () => ({
  ...jest.requireActual("@/backoffice/operators/operatorsApi"),
  getOperators: jest.fn(),
  disableOperator: jest.fn(),
}));

describe("When the server refuses to disable an access", () => {
  it("Then the reason is shown", async () => {
    jest.mocked(getOperators).mockResolvedValue(anOperatorList());
    jest.mocked(disableOperator).mockRejectedValue(
      new BackofficeRequestError(409, {
        status: 409,
        code: "backoffice.cannot_disable_yourself",
      }),
    );
    await renderBackofficeScreen(<OperatorsScreen />);

    await fireEvent.press(
      await screen.findByRole("button", {
        name: translate("backoffice.operators.disableOf", { name: "Lectora Dos" }),
      }),
    );
    await fireEvent.press(
      screen.getByRole("button", { name: translate("backoffice.operators.disable") }),
    );

    expect(
      await screen.findByText(translate("apiErrors.backoffice.cannot_disable_yourself")),
    ).toBeOnTheScreen();
  });
});
