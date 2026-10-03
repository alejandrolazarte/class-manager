import { isNewerVersionPublished } from "@/features/appUpdate/webAppVersion";
import { loadedEntryScriptPath, publishedPageWith } from "@/testing/publishedPages";

describe("When the published version is the loaded one", () => {
  it("Then no update is available", async () => {
    const fetchPublishedPage = jest.fn(async () => publishedPageWith(loadedEntryScriptPath));

    const isUpdateAvailable = await isNewerVersionPublished(
      loadedEntryScriptPath,
      fetchPublishedPage,
    );

    expect(isUpdateAvailable).toBe(false);
  });
});
