import { User } from "@/types";

// Save token and user info after login
export const saveAuth = (token: string, user: Omit<User, "id" | "isActive" | "createdAt">) => {
  localStorage.setItem("token", token);
  localStorage.setItem("user", JSON.stringify(user));
};

// Get current logged in user
export const getUser = (): { fullName: string; email: string; role: string } | null => {
  if (typeof window === "undefined") return null;
  const user = localStorage.getItem("user");
  if (!user) return null;
  const parsed = JSON.parse(user);
  return {
    fullName: parsed.fullName ?? "",
    email: parsed.email ?? "",
    role: parsed.role ?? "",
  };
};

// Check if user is logged in
export const isAuthenticated = (): boolean => {
  if (typeof window === "undefined") return false;
  return !!localStorage.getItem("token");
};

// Logout
export const logout = () => {
  localStorage.removeItem("token");
  localStorage.removeItem("user");
  window.location.href = "/login";
};

// Get redirect path based on role
export const getRoleRedirect = (role: string): string => {
  switch (role) {
    case "Admin": return "/admin";
    case "Teacher": return "/teacher";
    case "Student": return "/student";
    default: return "/login";
  }
};