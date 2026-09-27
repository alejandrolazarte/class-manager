import { z } from "zod";
import { findThemeContrastFailures } from "@/theme/themeContrast";
import { themeColorTokens } from "@/theme/themeColorTokens";
import { ThemeDefinition } from "@/theme/themes";

const hexColorPattern = /^#[0-9a-f]{6}$/i;
const hexColorMessage = "expected a #rrggbb color";
const pathSeparator = ".";

const themeColorsSchema = z.object(
  Object.fromEntries(
    themeColorTokens.map((token) => [
      token,
      z.string({ error: hexColorMessage }).regex(hexColorPattern, hexColorMessage),
    ]),
  ) as Record<(typeof themeColorTokens)[number], z.ZodString>,
);

const themeDefinitionSchema = z.object({
  light: themeColorsSchema,
  dark: themeColorsSchema,
});

export type ParsedThemeDefinition =
  { isValid: true; theme: ThemeDefinition } | { isValid: false; problems: string[] };

export function parseThemeDefinition(candidate: unknown): ParsedThemeDefinition {
  const parsed = themeDefinitionSchema.safeParse(candidate);
  if (!parsed.success) {
    return {
      isValid: false,
      problems: parsed.error.issues.map(
        (issue) => `${issue.path.join(pathSeparator)}: ${issue.message}`,
      ),
    };
  }
  const contrastFailures = findThemeContrastFailures(parsed.data);
  if (contrastFailures.length > 0) {
    return { isValid: false, problems: contrastFailures.map((failure) => failure.pair) };
  }
  return { isValid: true, theme: parsed.data };
}
