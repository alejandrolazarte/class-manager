import { isNewerVersionPublished } from "@/features/appUpdate/webAppVersion";
import {
  loadedEntryScriptPath,
  newerEntryScriptPath,
  publishedPageWith,
} from "@/testing/publishedPages";

describe("When a new version is published", () => {
  it("Then an update is available", async () => {
    const fetchPublishedPage = jest.fn(async () => publishedPageWith(newerEntryScriptPath));

    const isUpdateAvailable = await isNewerVersionPublished(
      loadedEntryScriptPath,
      fetchPublishedPage,
    );

    expect(isUpdateAvailable).toBe(true);
  });
});
