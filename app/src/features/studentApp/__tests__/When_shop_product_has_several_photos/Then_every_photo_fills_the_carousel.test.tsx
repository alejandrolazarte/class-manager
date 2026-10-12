import { fireEvent, render, screen } from "@testing-library/react-native";
import { ProductPhotoCarousel } from "@/features/studentApp/components/ProductPhotoCarousel";
import { buildCatalogImages } from "@/testing/catalogImageFactory";

const carouselSize = { width: 360, height: 360 };

describe("When shop product has several photos", () => {
  it("Then every photo fills the carousel", async () => {
    await render(
      <ProductPhotoCarousel
        images={buildCatalogImages(2)}
        sizeClassName="aspect-square"
        size="page"
      />,
    );

    await fireEvent(screen.getByTestId("photo-carousel"), "layout", {
      nativeEvent: { layout: carouselSize },
    });

    const pages = screen.getAllByTestId("photo-carousel-page");
    expect(pages).toHaveLength(2);
    pages.forEach((page) => expect(page).toHaveStyle(carouselSize));
  });
});
