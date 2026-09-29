import { httpClient } from "@/api/httpClient";
import {
  Product,
  RecordStockMovementRequest,
  SaveProductRequest,
  StockMovement,
} from "@/features/products/types";

const productsPath = "/api/products";
const activeSegment = "active";
const stockSegment = "stock";
const stockMovementsSegment = "stock-movements";
const includeInactiveParameters = { includeInactive: "true" };

function productPath(productId: string): string {
  return `${productsPath}/${encodeURIComponent(productId)}`;
}

export function listProducts(includeInactive: boolean): Promise<Product[]> {
  return includeInactive
    ? httpClient.get<Product[]>(productsPath, includeInactiveParameters)
    : httpClient.get<Product[]>(productsPath);
}

export function createProduct(request: SaveProductRequest): Promise<Product> {
  return httpClient.post<Product>(productsPath, request);
}

export function updateProduct(productId: string, request: SaveProductRequest): Promise<Product> {
  return httpClient.put<Product>(productPath(productId), request);
}

export function setProductActive(productId: string, isActive: boolean): Promise<Product> {
  return httpClient.put<Product>(`${productPath(productId)}/${activeSegment}`, { isActive });
}

export function recordStockMovement(
  productId: string,
  request: RecordStockMovementRequest,
): Promise<Product> {
  return httpClient.post<Product>(`${productPath(productId)}/${stockSegment}`, request);
}

export function listStockMovements(productId: string): Promise<StockMovement[]> {
  return httpClient.get<StockMovement[]>(`${productPath(productId)}/${stockMovementsSegment}`);
}
