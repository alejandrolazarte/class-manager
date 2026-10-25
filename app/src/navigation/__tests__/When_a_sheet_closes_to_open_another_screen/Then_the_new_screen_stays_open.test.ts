import { createBackDismissal } from "@/navigation/layerHistory";
import { FakeLayerHistory } from "@/testing/fakeLayerHistory";

describe("When a sheet closes to open another screen", () => {
  it("Then the new screen stays open", () => {
    const history = new FakeLayerHistory("/settings/branches");
    const layerEntry = createBackDismissal(history).open(jest.fn());
    layerEntry.removeAfterNavigation();
    history.navigateTo("/settings/plan");

    history.runPendingTasks();

    expect(history.currentPage).toBe("/settings/plan");
  });
});
