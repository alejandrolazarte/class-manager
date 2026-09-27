import { nodeFileSystem, nodeOperatingSystem, nodePath } from "@/testing/nodeFileSystem";

export function createWebExport(files: Record<string, string>): string {
  const outputDirectory = nodeFileSystem.mkdtempSync(
    nodePath.join(nodeOperatingSystem.tmpdir(), "web-export-"),
  );
  for (const [relativePath, content] of Object.entries(files)) {
    const filePath = nodePath.join(outputDirectory, relativePath);
    nodeFileSystem.mkdirSync(nodePath.dirname(filePath), { recursive: true });
    nodeFileSystem.writeFileSync(filePath, content);
  }
  return outputDirectory;
}

export function readExportedFile(outputDirectory: string, relativePath: string): string {
  return nodeFileSystem.readFileSync(nodePath.join(outputDirectory, relativePath), "utf8");
}
