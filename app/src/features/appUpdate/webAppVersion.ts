const publishedPagePath = "/";
const entryScriptPathPattern = /\/_expo\/static\/js\/web\/entry-[\w-]+\.js/;

type FetchPublishedPage = (path: string, requestInit: RequestInit) => Promise<Response>;

function entryScriptPathOf(text: string): string | null {
  return entryScriptPathPattern.exec(text)?.[0] ?? null;
}

export function loadedEntryScriptPath(): string | null {
  const scriptSources = Array.from(document.scripts, (script) => script.src);
  return scriptSources.map(entryScriptPathOf).find((path) => path !== null) ?? null;
}

export async function isNewerVersionPublished(
  currentEntryScriptPath: string,
  fetchPublishedPage: FetchPublishedPage = fetch,
): Promise<boolean> {
  try {
    const response = await fetchPublishedPage(publishedPagePath, { cache: "no-store" });
    if (!response.ok) {
      return false;
    }
    const publishedEntryScriptPath = entryScriptPathOf(await response.text());
    return publishedEntryScriptPath !== null && publishedEntryScriptPath !== currentEntryScriptPath;
  } catch {
    return false;
  }
}
