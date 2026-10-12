import { productColorSwatch } from "@/features/products/productColors";
import { ColorSwatch } from "@/ui/ColorSwatch";

interface ProductColorRowLabelProps {
  color: string;
}

export function ProductColorRowLabel({ color }: ProductColorRowLabelProps) {
  const swatch = productColorSwatch(color);
  return swatch === null ? null : <ColorSwatch swatch={swatch} />;
}
