import { ESLint } from "eslint";

const { join } = jest.requireActual("node:path");

const projectRoot = process.cwd();
const projectConfig = jest.requireActual("../../eslint.config.js");

export async function restrictedImportMessages(
  sourceCode: string,
  relativeFilePath: string,
): Promise<string[]> {
  const eslint = new ESLint({
    cwd: projectRoot,
    overrideConfigFile: true,
    overrideConfig: projectConfig,
  });
  const [result] = await eslint.lintText(sourceCode, {
    filePath: join(projectRoot, relativeFilePath),
  });
  return result.messages
    .filter((message) => message.ruleId === "no-restricted-imports")
    .map((message) => message.message);
}
