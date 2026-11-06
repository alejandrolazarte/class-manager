import { z } from "zod";
import { parseAmount } from "@/formatting/money";
import { translate } from "@/i18n/translate";

const maximumPercent = 100;
const percentDecimals = 2;
const noteMaximumLength = 200;

export function parsePercent(typedPercent: string): number | null {
  const percent = parseAmount(typedPercent);
  if (percent === null || percent < 0 || percent > maximumPercent) {
    return null;
  }
  return Number(percent.toFixed(percentDecimals)) === percent ? percent : null;
}

const percentShape = z
  .string()
  .refine(
    (typedPercent) => parsePercent(typedPercent) !== null,
    translate("backoffice.validation.salesFee"),
  );

export const planSalesFeeSchema = z.object({ percent: percentShape });

export const brandSalesFeeSchema = z.object({
  percent: percentShape,
  note: z.string().max(noteMaximumLength, translate("backoffice.validation.note")),
});

export type PlanSalesFeeFormValues = z.input<typeof planSalesFeeSchema>;

export type BrandSalesFeeFormValues = z.input<typeof brandSalesFeeSchema>;
