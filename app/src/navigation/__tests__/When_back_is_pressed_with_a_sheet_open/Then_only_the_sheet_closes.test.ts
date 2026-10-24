import { createBackDismissal } from "@/navigation/layerHistory";
import { FakeLayerHistory } from "@/testing/fakeLayerHistory";

describe("When back is pressed with a sheet open", () => {
  it("Then only the sheet closes", () => {
    const history = new FakeLayerHistory("/settings");
    const closeSheet = jest.fn();
    createBackDismissal(history).open(closeSheet);

    history.pressBack();

    expect(closeSheet).toHaveBeenCalledTimes(1);
    expect(history.navigationBackPresses).toEqual([]);
  });
});
