import { TiShell } from "@/components/layout/Shells";

export default function TiLayout({ children }: { children: React.ReactNode }) {
  return <TiShell>{children}</TiShell>;
}
