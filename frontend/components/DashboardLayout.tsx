"use client";

import { useEffect, useState } from "react";
import { useRouter, usePathname } from "next/navigation";
import { getUser, logout, isAuthenticated } from "@/lib/auth";

interface NavItem {
  label: string;
  href: string;
}

const navItems: Record<string, NavItem[]> = {
  Admin: [
    { label: "Users", href: "/admin/users" },
    { label: "Classes", href: "/admin/classes" },
    { label: "Assignments", href: "/admin/assignments" },
  ],
  Teacher: [
    { label: "Assignments", href: "/teacher/assignments" },
  ],
  Student: [
    { label: "Assignments", href: "/student/assignments" },
  ],
};

export default function DashboardLayout({ children }: { children: React.ReactNode }) {
  const router = useRouter();
  const pathname = usePathname();
  const [user, setUser] = useState<{ fullName: string; role: string; email: string } | null>(null);
  const [mounted, setMounted] = useState(false);

  useEffect(() => {
    setMounted(true);
    if (!isAuthenticated()) {
      router.push("/login");
      return;
    }
    setUser(getUser());
  }, [router]);

  // Don't render anything until client-side mount
  if (!mounted) return null;
  if (!user) return null;

  const nav = navItems[user.role] || [];

  return (
    <div className="min-h-screen flex bg-gray-50">
      <aside className="w-56 bg-white border-r border-gray-200 flex flex-col">
        <div className="px-6 py-5 border-b border-gray-200">
          <h1 className="text-base font-bold text-blue-600">AssignmentSys</h1>
          <p className="text-xs text-gray-400 mt-0.5">{user.role}</p>
        </div>

        <nav className="flex-1 px-3 py-4 space-y-1">
          {nav.map((item) => (
            <a
              key={item.href}
              href={item.href}
              className={`block px-3 py-2 rounded-lg text-sm font-medium transition ${
                pathname.startsWith(item.href)
                  ? "bg-blue-50 text-blue-700"
                  : "text-gray-600 hover:bg-gray-100"
              }`}
            >
              {item.label}
            </a>
          ))}
        </nav>

        <div className="px-4 py-4 border-t border-gray-200">
          <p className="text-sm font-medium text-gray-700 truncate">{user.fullName}</p>
          <p className="text-xs text-gray-400 truncate">{user.email}</p>
          <button
            onClick={logout}
            className="mt-3 w-full text-xs text-red-500 hover:text-red-700 text-left"
          >
            Sign out
          </button>
        </div>
      </aside>

      <main className="flex-1 p-6 overflow-auto">
        {children}
      </main>
    </div>
  );
}