import { RhShell } from "@/components/layout/Shells";

export default function RhLayout({ children }: { children: React.ReactNode }) {
  return <RhShell>{children}</RhShell>;
}
