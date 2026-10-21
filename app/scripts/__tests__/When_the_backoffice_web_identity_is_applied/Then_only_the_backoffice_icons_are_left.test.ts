import { nodeFileSystem, nodePath } from "@/testing/nodeFileSystem";
import { createWebExport } from "../createWebExport";
import { appIndexHtml, backofficeIdentitySource } from "../createBackofficeIdentitySource";

const { applyBackofficeWebIdentity } = jest.requireActual("../../applyBackofficeWebIdentity.js");

describe("When the backoffice web identity is applied", () => {
  let outputDirectory: string;
  let sourceDirectory: string;

  beforeEach(() => {
    outputDirectory = createWebExport({
      "index.html": appIndexHtml,
      "icons/icon-192.png": "app icon",
      "icons/app-only.png": "app icon",
    });
    sourceDirectory = backofficeIdentitySource();
  });

  afterEach(() => {
    nodeFileSystem.rmSync(outputDirectory, { recursive: true, force: true });
    nodeFileSystem.rmSync(sourceDirectory, { recursive: true, force: true });
  });

  it("Then only the backoffice icons are left", () => {
    applyBackofficeWebIdentity(outputDirectory, sourceDirectory);

    const iconsDirectory = nodePath.join(outputDirectory, "icons");
    expect(nodeFileSystem.readdirSync(iconsDirectory)).toEqual(["icon-192.png"]);
    expect(nodeFileSystem.readFileSync(nodePath.join(iconsDirectory, "icon-192.png"), "utf8")).toBe(
      "backoffice icon",
    );
  });
});
