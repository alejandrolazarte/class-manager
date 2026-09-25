const coldTransformTestTimeoutMilliseconds = 20000;

module.exports = {
  preset: "jest-expo",
  setupFilesAfterEnv: ["<rootDir>/jest.setup.ts"],
  testMatch: ["**/__tests__/**/*.test.ts?(x)"],
  testTimeout: coldTransformTestTimeoutMilliseconds,
  moduleNameMapper: {
    "^@/(.*)$": "<rootDir>/src/$1",
  },
};
