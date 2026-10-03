import { useEffect } from "react";

export const routerMock = {
  push: jest.fn(),
  replace: jest.fn(),
  navigate: jest.fn(),
  back: jest.fn(),
  dismiss: jest.fn(),
  canDismiss: jest.fn(() => true),
  canGoBack: jest.fn(() => true),
  setParams: jest.fn(),
};

export const searchParametersMock: { current: Record<string, string> } = { current: {} };

export const segmentsMock: { current: string[] } = { current: [] };

export function resetExpoRouterMock(): void {
  Object.values(routerMock).forEach((mockFunction) => mockFunction.mockClear());
  searchParametersMock.current = {};
  segmentsMock.current = [];
}

export function RedirectMock({ href }: { href: unknown }): null {
  useEffect(() => {
    routerMock.replace(href);
  }, [href]);
  return null;
}
