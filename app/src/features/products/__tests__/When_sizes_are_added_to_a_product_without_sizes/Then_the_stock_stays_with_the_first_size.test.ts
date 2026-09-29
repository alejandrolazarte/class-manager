import { toProductFormValues, toSaveProductRequest } from "@/features/products/productSchema";
import { buildProduct } from "@/testing/productFactory";

describe("When sizes are added to a product without sizes", () => {
  it("Then the stock stays with the first size", () => {
    const product = buildProduct();

    const request = toSaveProductRequest(
      { ...toProductFormValues(product), variantNames: "Único, XL" },
      product,
    );

    expect(request.variants).toEqual([
      { id: product.variants[0].id, name: "Único" },
      { id: null, name: "XL" },
    ]);
  });
});
