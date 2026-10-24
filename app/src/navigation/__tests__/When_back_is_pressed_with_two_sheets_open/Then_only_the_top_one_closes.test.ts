import { createBackDismissal } from "@/navigation/layerHistory";
import { FakeLayerHistory } from "@/testing/fakeLayerHistory";

describe("When back is pressed with two sheets open", () => {
  it("Then only the top one closes", () => {
    const history = new FakeLayerHistory("/settings");
    const backDismissal = createBackDismissal(history);
    const closeBottomSheet = jest.fn();
    const closeTopSheet = jest.fn();
    backDismissal.open(closeBottomSheet);
    backDismissal.open(closeTopSheet);

    history.pressBack();

    expect(closeTopSheet).toHaveBeenCalledTimes(1);
    expect(closeBottomSheet).not.toHaveBeenCalled();
  });
});
