import { routes } from "@/navigation/routes";
import { nodeFileSystem, nodePath } from "@/testing/nodeFileSystem";

const { existsSync, readFileSync, readdirSync, statSync } = nodeFileSystem;
const { join, relative } = nodePath;
const sourceDirectoryMarker = "/src/";
const testPath = expect.getState().testPath ?? "";
const appRoot = testPath.slice(0, testPath.lastIndexOf(sourceDirectoryMarker));
const tabsDirectory = join(appRoot, "app/(tabs)");
const sourceDirectory = join(appRoot, "src");
const followedSourceDirectories = ["features", "ui"].map((directoryName) =>
  join(sourceDirectory, directoryName),
);
const layoutFileName = "_layout.tsx";
const routeFileExtension = ".tsx";
const indexSegment = "index";
const sourceFileCandidates = [".ts", ".tsx", "/index.ts", "/index.tsx"];
const sourceAliasImportPattern = /from\s+"@\/([^"]+)"/g;
const routeUsagePattern = /\broutes\.(\w+)(?:\(\s*"(\w+)")?/g;
const dynamicSegmentPattern = /^\[.+\]$/;
const pathSeparatorPattern = /[/?]/;
const querySeparator = "?";

type RouteKey = keyof typeof routes;

export interface TabLink {
  originTab: string;
  routeKey: string;
  path: string;
  sourceFile: string;
}

export function listTabNames(): string[] {
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

function toPath(routeKey: RouteKey, firstArgument: string, originTab: string): string {
  const route: unknown = routes[routeKey];
  return typeof route === "function"
    ? (route as (...routeArguments: string[]) => string)(firstArgument, originTab, originTab)
    : (route as string);
}

function listLinksOf(sourceFile: string, originTab: string): TabLink[] {
  const sourceText = readFileSync(sourceFile, "utf8");
  return [...sourceText.matchAll(routeUsagePattern)]
    .filter(([, routeKey]) => routeKey in routes)
    .map(([, routeKey, literalArgument]) => ({
      originTab,
      routeKey,
      path: toPath(routeKey as RouteKey, literalArgument ?? originTab, originTab),
      sourceFile: relative(appRoot, sourceFile),
    }));
}

export function findTabLinks(): TabLink[] {
  return listTabNames().flatMap((originTab) =>
    listRouteFiles(join(tabsDirectory, originTab))
      .flatMap(listReachableFiles)
      .flatMap((sourceFile) => listLinksOf(sourceFile, originTab)),
  );
}

export function targetTabOf(path: string): string {
  return path.split(pathSeparatorPattern)[1];
}

export function isTabRoot(path: string): boolean {
  return path.split(querySeparator)[0] === `/${targetTabOf(path)}`;
}

function toScreenSegments(routeFile: string): string[] {
  const segments = relative(tabsDirectory, routeFile)
    .slice(0, -routeFileExtension.length)
    .split("/");
  return segments.at(-1) === indexSegment ? segments.slice(0, -1) : segments;
}

export function hasScreen(path: string): boolean {
  const tabName = targetTabOf(path);
  const pathSegments = path.split("?")[0].split("/").slice(1);
  return listRouteFiles(join(tabsDirectory, tabName))
    .map(toScreenSegments)
    .some(
      (screenSegments) =>
        screenSegments.length === pathSegments.length &&
        screenSegments.every(
          (screenSegment, position) =>
            dynamicSegmentPattern.test(screenSegment) || screenSegment === pathSegments[position],
        ),
    );
}
