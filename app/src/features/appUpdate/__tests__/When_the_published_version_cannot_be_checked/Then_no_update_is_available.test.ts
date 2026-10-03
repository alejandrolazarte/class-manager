import { isNewerVersionPublished } from "@/features/appUpdate/webAppVersion";
import { loadedEntryScriptPath } from "@/testing/publishedPages";

describe("When the published version cannot be checked", () => {
  it("Then no update is available", async () => {
    const fetchPublishedPage = jest.fn(async () => {
      throw new TypeError("Network request failed");
    });

    const isUpdateAvailable = await isNewerVersionPublished(
      loadedEntryScriptPath,
      fetchPublishedPage,
    );

    expect(isUpdateAvailable).toBe(false);
  });
});
