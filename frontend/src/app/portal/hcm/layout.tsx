"use client";

import { useEffect } from "react";
import { useRouter } from "next/navigation";
import { getSession, isRhAdmin } from "@/lib/auth";

export default function HcmLayout({ children }: { children: React.ReactNode }) {
  const router = useRouter();

  useEffect(() => {
    const session = getSession();
    if (!session) {
      router.replace("/login");
      return;
    }
    if (!isRhAdmin(session)) {
      router.replace("/portal");
    }
  }, [router]);

  return <>{children}</>;
}
