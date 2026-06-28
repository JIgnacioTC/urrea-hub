const SIDEBAR_COLLAPSED_KEY = "urrea_sidebar_collapsed";

export function getSidebarCollapsed(): boolean {
  if (typeof window === "undefined") return false;
  return localStorage.getItem(SIDEBAR_COLLAPSED_KEY) === "1";
}

export function setSidebarCollapsed(collapsed: boolean) {
  localStorage.setItem(SIDEBAR_COLLAPSED_KEY, collapsed ? "1" : "0");
}

export const SIDEBAR_WIDTH_EXPANDED = "18rem";
export const SIDEBAR_WIDTH_COLLAPSED = "4.5rem";
