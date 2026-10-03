export const loadedEntryScriptPath = "/_expo/static/js/web/entry-loaded0123456789abcdef.js";
export const newerEntryScriptPath = "/_expo/static/js/web/entry-newer0123456789abcdef.js";

export function publishedPageWith(entryScriptPath: string): Response {
  return new Response(
    `<!doctype html><html><body><div id="root"></div><script src="${entryScriptPath}" defer></script></body></html>`,
  );
}
