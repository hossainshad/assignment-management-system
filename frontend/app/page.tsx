"use client";

import { useEffect } from "react";
import { useRouter } from "next/navigation";

export default function Home() {
  const router = useRouter();

  useEffect(() => {
    const token = localStorage.getItem("token");
    const user = localStorage.getItem("user");

    if (token && user) {
      const parsed = JSON.parse(user);
      const role = parsed.role;
      if (role === "Admin") router.push("/admin");
      else if (role === "Teacher") router.push("/teacher");
      else if (role === "Student") router.push("/student");
      else router.push("/login");
    } else {
      router.push("/login");
    }
  }, [router]);

  return (
    <div className="min-h-screen flex items-center justify-center">
      <p className="text-gray-400 text-sm">Redirecting...</p>
    </div>
  );
}