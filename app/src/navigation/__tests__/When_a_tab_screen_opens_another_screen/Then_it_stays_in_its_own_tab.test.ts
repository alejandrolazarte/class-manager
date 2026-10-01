import { routes } from "@/navigation/routes";
import { nodeFileSystem, nodePath } from "@/testing/nodeFileSystem";

const { existsSync, readFileSync, readdirSync, statSync } = nodeFileSystem;
const { join, relative } = nodePath;
const appRootFromTestFile = "../../../../..";
const appRoot = join(expect.getState().testPath ?? "", appRootFromTestFile);
const tabsDirectory = join(appRoot, "app/(tabs)");
const sourceDirectory = join(appRoot, "src");
const followedSourceDirectories = ["features", "ui"].map((directoryName) =>
  join(sourceDirectory, directoryName),
);
const layoutFileName = "_layout.tsx";
const routeFileExtension = ".tsx";
const sourceFileCandidates = [".ts", ".tsx", "/index.ts", "/index.tsx"];
const sourceAliasImportPattern = /from\s+"@\/([^"]+)"/g;
const routeUsagePattern = /\broutes\.(\w+)/g;
const placeholderArgument = "placeholder";

const knownCrossTabLinks = [
  "classes → newInstructor",
  "fees → clientDetail",
  "fees → defaultMonthlyFee",
  "students → classPacks",
  "students → recordPayment",
];

type RouteKey = keyof typeof routes;

function listTabNames(): string[] {
  return readdirSync(tabsDirectory).filter(
    (entryName) =>
      statSync(join(tabsDirectory, entryName)).isDirectory() &&
      existsSync(join(tabsDirectory, entryName, layoutFileName)),
  );
}

function listRouteFiles(currentDirectory: string): string[] {
  return readdirSync(currentDirectory).flatMap((entryName) => {
    const entryPath = join(currentDirectory, entryName);
    if (statSync(entryPath).isDirectory()) {
      return listRouteFiles(entryPath);
    }
    return entryName.endsWith(routeFileExtension) && entryName !== layoutFileName
      ? [entryPath]
      : [];
  });
}

function resolveSourceFile(importPath: string): string | undefined {
  const basePath = join(sourceDirectory, importPath);
  if (!followedSourceDirectories.some((directory) => basePath.startsWith(directory))) {
    return undefined;
  }
  return sourceFileCandidates
    .map((candidate) => `${basePath}${candidate}`)
    .find((candidatePath) => existsSync(candidatePath));
}

function listReachableFiles(entryFile: string): string[] {
  const visitedFiles = new Set<string>();
  const pendingFiles = [entryFile];
  while (pendingFiles.length > 0) {
    const currentFile = pendingFiles.pop() as string;
    if (visitedFiles.has(currentFile)) {
      continue;
    }
    visitedFiles.add(currentFile);
    const sourceText = readFileSync(currentFile, "utf8");
    for (const [, importPath] of sourceText.matchAll(sourceAliasImportPattern)) {
      const importedFile = resolveSourceFile(importPath);
      if (importedFile) {
        pendingFiles.push(importedFile);
      }
    }
  }
  return [...visitedFiles];
}

function listUsedRouteKeys(sourceFile: string): RouteKey[] {
  const sourceText = readFileSync(sourceFile, "utf8");
  return [...sourceText.matchAll(routeUsagePattern)]
    .map(([, routeKey]) => routeKey)
    .filter((routeKey): routeKey is RouteKey => routeKey in routes);
}

function toPath(routeKey: RouteKey): string {
  const route: unknown = routes[routeKey];
  return typeof route === "function"
    ? (route as (...routeArguments: string[]) => string)(placeholderArgument, placeholderArgument)
    : (route as string);
}

function leavesTab(path: string, originTab: string, tabNames: string[]): boolean {
  const targetTab = path.split(/[/?]/)[1];
  const isTabRoot = path === `/${targetTab}`;
  return tabNames.includes(targetTab) && targetTab !== originTab && !isTabRoot;
}

function findCrossTabLinks(tabNames: string[]): Map<string, string> {
  const crossTabLinks = new Map<string, string>();
  for (const originTab of tabNames) {
    for (const routeFile of listRouteFiles(join(tabsDirectory, originTab))) {
      for (const sourceFile of listReachableFiles(routeFile)) {
        for (const routeKey of listUsedRouteKeys(sourceFile)) {
          const path = toPath(routeKey);
          if (leavesTab(path, originTab, tabNames)) {
            crossTabLinks.set(
              `${originTab} → ${routeKey}`,
              `${path} (${relative(appRoot, sourceFile)})`,
            );
          }
        }
      }
    }
  }
  return crossTabLinks;
}

describe("When a tab screen opens another screen", () => {
  it("Then it stays in its own tab", () => {
    const tabNames = listTabNames();

    const crossTabLinks = findCrossTabLinks(tabNames);
    const newCrossTabLinks = [...crossTabLinks]
      .filter(([link]) => !knownCrossTabLinks.includes(link))
      .map(([link, location]) => `${link}: ${location}`);
    const fixedKnownLinks = knownCrossTabLinks.filter((link) => !crossTabLinks.has(link));

    expect(tabNames.length).toBeGreaterThan(0);
    expect(newCrossTabLinks).toEqual([]);
    expect(fixedKnownLinks).toEqual([]);
  });
});
