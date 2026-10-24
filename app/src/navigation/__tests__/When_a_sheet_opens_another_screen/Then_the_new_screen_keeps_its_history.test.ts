import { createBackDismissal } from "@/navigation/layerHistory";
import { FakeLayerHistory } from "@/testing/fakeLayerHistory";

describe("When a sheet opens another screen", () => {
  it("Then the new screen keeps its history", () => {
    const history = new FakeLayerHistory("/settings");
    const release = createBackDismissal(history).open(jest.fn());
    history.navigateTo("/settings/plan");

    release();

    expect(history.currentPage).toBe("/settings/plan");
  });
});
