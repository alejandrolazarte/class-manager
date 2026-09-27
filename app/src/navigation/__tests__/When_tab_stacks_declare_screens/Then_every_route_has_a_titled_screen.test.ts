import { isValidElement, ReactElement, ReactNode } from "react";

interface FileSystem {
  existsSync: (path: string) => boolean;
  readdirSync: (path: string) => string[];
  statSync: (path: string) => { isDirectory: () => boolean };
}

interface PathUtilities {
  join: (...paths: string[]) => string;
  relative: (from: string, to: string) => string;
}

const { existsSync, readdirSync, statSync }: FileSystem = jest.requireActual("node:fs");
const { join, relative }: PathUtilities = jest.requireActual("node:path");
const appRootFromTestFile = "../../../../..";

const tabsDirectory = join(expect.getState().testPath ?? "", appRootFromTestFile, "app/(tabs)");
const layoutFileName = "_layout.tsx";
const routeFileExtension = ".tsx";

interface ScreenProps {
  name: string;
  options?: { title?: string };
}

function listRouteNames(stackDirectory: string, currentDirectory = stackDirectory): string[] {
  return readdirSync(currentDirectory).flatMap((entryName) => {
    const entryPath = join(currentDirectory, entryName);
    if (statSync(entryPath).isDirectory()) {
      return listRouteNames(stackDirectory, entryPath);
    }
    if (entryName === layoutFileName || !entryName.endsWith(routeFileExtension)) {
      return [];
    }
    return [relative(stackDirectory, entryPath).slice(0, -routeFileExtension.length)];
  });
}

function listDeclaredScreens(stackDirectory: string): ScreenProps[] {
  const renderLayout: () => ReactElement<{ children: ReactNode }> = jest.requireActual(
    join(stackDirectory, layoutFileName),
  ).default;
  const children = [renderLayout().props.children].flat();
  return children
    .filter((child): child is ReactElement<ScreenProps> => isValidElement(child))
    .map((child) => child.props);
}

function findStackProblems(stackName: string): string[] {
  const stackDirectory = join(tabsDirectory, stackName);
  const declaredScreens = listDeclaredScreens(stackDirectory);
  const declaredNames = declaredScreens.map((screen) => screen.name);
  const routeNames = listRouteNames(stackDirectory);
  return [
    ...routeNames
      .filter((routeName) => !declaredNames.includes(routeName))
      .map((routeName) => `${stackName}: route "${routeName}" has no Stack.Screen`),
    ...declaredNames
      .filter((declaredName) => !routeNames.includes(declaredName))
      .map((declaredName) => `${stackName}: Stack.Screen "${declaredName}" matches no route`),
    ...declaredScreens
      .filter((screen) => !screen.options?.title)
      .map((screen) => `${stackName}: Stack.Screen "${screen.name}" has no title`),
  ];
}

describe("When tab stacks declare screens", () => {
  it("Then every route has a titled screen", () => {
    const stackNames = readdirSync(tabsDirectory).filter(
      (entryName) =>
        statSync(join(tabsDirectory, entryName)).isDirectory() &&
        existsSync(join(tabsDirectory, entryName, layoutFileName)),
    );

    const problems = stackNames.flatMap(findStackProblems);

    expect(stackNames.length).toBeGreaterThan(0);
    expect(problems).toEqual([]);
  });
});
