import { createBackDismissal } from "@/navigation/layerHistory";
import { FakeLayerHistory } from "@/testing/fakeLayerHistory";

describe("When a sheet closes by itself", () => {
  it("Then its history entry is removed", () => {
    const history = new FakeLayerHistory("/settings");
    const release = createBackDismissal(history).open(jest.fn());

    release();

    expect(history.currentLayerKey()).toBeUndefined();
    expect(history.navigationBackPresses).toEqual([]);
  });
});
