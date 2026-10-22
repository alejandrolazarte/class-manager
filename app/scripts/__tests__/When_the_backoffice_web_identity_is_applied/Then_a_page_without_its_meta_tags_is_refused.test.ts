import { nodeFileSystem } from "@/testing/nodeFileSystem";
import { createWebExport } from "../createWebExport";
import { backofficeIdentitySource } from "../createBackofficeIdentitySource";

const { applyBackofficeWebIdentity } = jest.requireActual("../../applyBackofficeWebIdentity.js");

describe("When the backoffice web identity is applied", () => {
  let outputDirectory: string;
  let sourceDirectory: string;

  beforeEach(() => {
    outputDirectory = createWebExport({ "index.html": "<html><head></head></html>" });
    sourceDirectory = backofficeIdentitySource();
  });

  afterEach(() => {
    nodeFileSystem.rmSync(outputDirectory, { recursive: true, force: true });
    nodeFileSystem.rmSync(sourceDirectory, { recursive: true, force: true });
  });

  it("Then a page without its meta tags is refused", () => {
    expect(() => applyBackofficeWebIdentity(outputDirectory, sourceDirectory)).toThrow(
      "theme-color",
    );
  });
});
