import { toProductFormValues, toSaveProductRequest } from "@/features/products/productSchema";
import { buildProduct } from "@/testing/productFactory";

describe("When colors are added to a product with sizes", () => {
  it("Then each size keeps its stock in the first color", () => {
    const product = buildProduct({
      variants: [
        { id: "variant-s", size: "S", color: null, stock: 2 },
        { id: "variant-m", size: "M", color: null, stock: 1 },
      ],
    });

    const request = toSaveProductRequest(
      { ...toProductFormValues(product), colors: ["black", "white"] },
      product,
    );

    expect(request.variants).toEqual([
      { id: "variant-s", size: "S", color: "black" },
      { id: null, size: "S", color: "white" },
      { id: "variant-m", size: "M", color: "black" },
      { id: null, size: "M", color: "white" },
    ]);
  });
});
