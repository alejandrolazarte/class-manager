import { hasTranslation } from "@/i18n/translate";
import { nodeFileSystem, nodePath } from "@/testing/nodeFileSystem";

const appDirectoryMarker = "/app/src/";
const testPath = expect.getState().testPath ?? "";
const repositoryDirectory = testPath.slice(0, testPath.lastIndexOf(appDirectoryMarker));
const featuresFilePath = nodePath.join(
  repositoryDirectory,
  "src/Core/Domain/Subscriptions/Features.cs",
);
const featureCodePattern = /public const string \w+ = "([^"]+)";/g;

function listFeatureCodes(): string[] {
  const featuresSource = nodeFileSystem.readFileSync(featuresFilePath, "utf8");
  return [...featuresSource.matchAll(featureCodePattern)].map((match) => match[1] ?? "");
}

describe("When the api defines a feature", () => {
  it("Then it has a label", () => {
    const featureCodes = listFeatureCodes();

    expect(featureCodes.length).toBeGreaterThan(0);
    expect(
      featureCodes.filter((featureCode) => !hasTranslation(`subscriptions.feature.${featureCode}`)),
    ).toEqual([]);
  });
});
