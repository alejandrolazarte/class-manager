import { useMutation } from "@tanstack/react-query";
import { fireEvent, screen } from "@testing-library/react-native";
import { ApiError } from "@/api/apiErrors";
import { translate } from "@/i18n/translate";
import { Button } from "@/ui/Button";
import { renderWithProviders } from "@/testing/renderWithProviders";

const forbiddenStatus = 403;
const saveLabel = "save";

function RefusedSave() {
  const save = useMutation({
    mutationFn: () =>
      Promise.reject(
        new ApiError(forbiddenStatus, { code: "feature.limit_reached", feature: "team" }),
      ),
  });
  return <Button label={saveLabel} onPress={() => save.mutate()} />;
}

describe("When a change is refused by the plan", () => {
  it("Then the plan is offered", async () => {
    await renderWithProviders(<RefusedSave />);

    await fireEvent.press(screen.getByRole("button", { name: saveLabel }));

    expect(await screen.findByText(translate("subscriptions.locked.team"))).toBeTruthy();
  });
});
