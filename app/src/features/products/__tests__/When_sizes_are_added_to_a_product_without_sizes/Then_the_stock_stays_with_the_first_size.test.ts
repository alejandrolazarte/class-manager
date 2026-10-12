import { toProductFormValues, toSaveProductRequest } from "@/features/products/productSchema";
import { buildProduct } from "@/testing/productFactory";

describe("When sizes are added to a product without sizes", () => {
  it("Then the stock stays with the first size", () => {
    const product = buildProduct();

    const request = toSaveProductRequest(
      { ...toProductFormValues(product), sizes: ["Único", "XL"] },
      product,
    );

    expect(request.variants).toEqual([
      { id: product.variants[0].id, size: "Único", color: null },
      { id: null, size: "XL", color: null },
    ]);
  });
});
