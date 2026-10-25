import { createBackDismissal } from "@/navigation/layerHistory";
import { FakeLayerHistory } from "@/testing/fakeLayerHistory";

describe("When a sheet closes by itself", () => {
  it("Then its history entry is removed", () => {
    const history = new FakeLayerHistory("/settings");
    const layerEntry = createBackDismissal(history).open(jest.fn());

    layerEntry.removeAfterNavigation();
    history.runPendingTasks();

    expect(history.currentLayerKey()).toBeUndefined();
    expect(history.navigationBackPresses).toEqual([]);
  });
});
