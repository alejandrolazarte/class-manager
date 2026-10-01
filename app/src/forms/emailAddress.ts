import { z } from "zod";

export const emailAddressPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/u;

export function emailAddressSchema(invalidMessage: string) {
  return z.email({ pattern: emailAddressPattern, error: invalidMessage });
}
