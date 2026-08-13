"use client";
import { useEffect } from "react";
import { useRouter } from "next/navigation";

export default function TeacherHome() {
  const router = useRouter();
  useEffect(() => { router.push("/teacher/assignments"); }, [router]);
  return null;
}