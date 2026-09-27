const {
  existsSync,
  readdirSync,
  readFileSync,
  renameSync,
  statSync,
  writeFileSync,
} = require("node:fs");
const { join } = require("node:path");

const nodeModulesAssetsPath = "assets/node_modules/";
const vendorAssetsPath = "assets/vendor/";
const textFileExtensions = [".html", ".js", ".css", ".json", ".map"];
const defaultOutputDirectory = "dist";

function listFiles(directory) {
  return readdirSync(directory).flatMap((entryName) => {
    const entryPath = join(directory, entryName);
    return statSync(entryPath).isDirectory() ? listFiles(entryPath) : [entryPath];
  });
}

function isTextFile(filePath) {
  return textFileExtensions.some((extension) => filePath.endsWith(extension));
}

function relocateNodeModulesAssets(outputDirectory) {
  const nodeModulesAssetsDirectory = join(outputDirectory, nodeModulesAssetsPath);
  if (!existsSync(nodeModulesAssetsDirectory)) {
    return;
  }
  const vendorAssetsDirectory = join(outputDirectory, vendorAssetsPath);
  if (existsSync(vendorAssetsDirectory)) {
    throw new Error(
      `${vendorAssetsDirectory} already exists; cannot relocate node_modules assets.`,
    );
  }
  renameSync(nodeModulesAssetsDirectory, vendorAssetsDirectory);
  for (const filePath of listFiles(outputDirectory).filter(isTextFile)) {
    const content = readFileSync(filePath, "utf8");
    if (content.includes(nodeModulesAssetsPath)) {
      writeFileSync(filePath, content.split(nodeModulesAssetsPath).join(vendorAssetsPath));
    }
  }
}

if (require.main === module) {
  const outputDirectory = process.argv[2] ?? defaultOutputDirectory;
  relocateNodeModulesAssets(outputDirectory);
  console.log(`Moved ${nodeModulesAssetsPath} to ${vendorAssetsPath} in ${outputDirectory}`);
}

module.exports = { relocateNodeModulesAssets };
