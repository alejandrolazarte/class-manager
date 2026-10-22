import { fireEvent, screen } from "@testing-library/react-native";
import { EnableOperatorScreen } from "@/backoffice/operators/EnableOperatorScreen";
import { getOperators } from "@/backoffice/operators/operatorsApi";
import { translate } from "@/i18n/translate";
import { anOperatorList } from "@/testing/backofficeOperators";
import { renderBackofficeScreen } from "@/testing/renderBackofficeScreen";

jest.mock("@/backoffice/operators/operatorsApi", () => ({
  ...jest.requireActual("@/backoffice/operators/operatorsApi"),
  getOperators: jest.fn(),
}));

describe("When no person is chosen", () => {
  it("Then enable stays disabled", async () => {
    jest.mocked(getOperators).mockResolvedValue(anOperatorList());
    await renderBackofficeScreen(<EnableOperatorScreen accessRequestId="request-1" />);

    await fireEvent.press(await screen.findByText(translate("roles.PlatformOperator")));

    expect(
      screen.getByRole("button", { name: translate("backoffice.enableOperator.submit") }),
    ).toBeDisabled();
  });
});
