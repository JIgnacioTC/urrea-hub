import { AdminTiShell } from "@/components/layout/Shells";

export default function AdminTiLayout({ children }: { children: React.ReactNode }) {
  return <AdminTiShell>{children}</AdminTiShell>;
}
