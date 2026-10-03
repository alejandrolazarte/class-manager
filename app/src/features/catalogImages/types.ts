export interface CatalogImage {
  id: string;
  url: string;
}

export interface CatalogImageOwner {
  images: CatalogImage[];
}
