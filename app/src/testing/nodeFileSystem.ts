interface NodeFileSystem {
  existsSync: (path: string) => boolean;
  mkdirSync: (path: string, options?: { recursive?: boolean }) => void;
  mkdtempSync: (prefix: string) => string;
  readdirSync: (path: string) => string[];
  readFileSync: (path: string, encoding: "utf8") => string;
  rmSync: (path: string, options?: { recursive?: boolean; force?: boolean }) => void;
  statSync: (path: string) => { isDirectory: () => boolean };
  writeFileSync: (path: string, content: string) => void;
}

interface NodePath {
  dirname: (path: string) => string;
  join: (...paths: string[]) => string;
  relative: (from: string, to: string) => string;
}

interface NodeOperatingSystem {
  tmpdir: () => string;
}

export const nodeFileSystem: NodeFileSystem = jest.requireActual("node:fs");
export const nodePath: NodePath = jest.requireActual("node:path");
export const nodeOperatingSystem: NodeOperatingSystem = jest.requireActual("node:os");
