import { useState } from "react";
import { ScrollView, View } from "react-native";
import { CatalogImage as CatalogImageView } from "@/features/catalogImages/components/CatalogImage";
import { CatalogImage } from "@/features/catalogImages/types";
import { ProductPhoto } from "@/features/family/components/ProductPhoto";

interface ProductPhotoCarouselProps {
  images: CatalogImage[];
  heightClassName: string;
}

export function ProductPhotoCarousel({ images, heightClassName }: ProductPhotoCarouselProps) {
  const [width, setWidth] = useState(0);
  if (images.length <= 1) {
    return <ProductPhoto imageUrl={images[0]?.url ?? null} heightClassName={heightClassName} />;
  }
  return (
    <View onLayout={(event) => setWidth(event.nativeEvent.layout.width)}>
      <ScrollView horizontal pagingEnabled showsHorizontalScrollIndicator={false}>
        {images.map((image) => (
          <View key={image.id} style={{ width }}>
            <CatalogImageView
              imageUri={image.url}
              placeholderIcon="products"
              className={`w-full ${heightClassName}`}
            />
          </View>
        ))}
      </ScrollView>
    </View>
  );
}
