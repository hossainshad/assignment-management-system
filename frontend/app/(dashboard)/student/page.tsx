"use client";
import { useEffect } from "react";
import { useRouter } from "next/navigation";

export default function StudentHome() {
  const router = useRouter();
  useEffect(() => { router.push("/student/assignments"); }, [router]);
  return null;
}