import Link from "next/link";
import { modules } from "@/lib/modules";

export function AppShell({ children }: { children: React.ReactNode }) {
  return (
    <div className="flex min-h-screen">
      <Sidebar />
      <div className="flex min-h-screen flex-1 flex-col">{children}</div>
    </div>
  );
}

export function Sidebar() {
  return (
    <aside className="flex h-full w-64 flex-col border-r border-slate-200 bg-slate-950 text-white">
      <div className="border-b border-slate-800 px-5 py-6">
        <Link href="/" className="block">
          <p className="text-xs uppercase tracking-[0.2em] text-slate-400">Plataforma RH</p>
          <h1 className="mt-1 text-xl font-semibold">URREA Hub</h1>
        </Link>
      </div>
      <nav className="flex-1 overflow-y-auto px-3 py-4">
        <ul className="space-y-1">
          {modules.map((module) => (
            <li key={module.id}>
              <Link
                href={module.href}
                className="block rounded-lg px-3 py-2 text-sm text-slate-300 transition hover:bg-slate-900 hover:text-white"
              >
                {module.name}
              </Link>
            </li>
          ))}
        </ul>
      </nav>
    </aside>
  );
}

export function Topbar() {
  return (
    <header className="flex items-center justify-between border-b border-slate-200 bg-white px-6 py-4">
      <div>
        <p className="text-sm text-slate-500">MVP · Recursos Humanos</p>
        <h2 className="text-lg font-semibold text-slate-900">Centro de operaciones</h2>
      </div>
      <span className="rounded-full bg-emerald-50 px-3 py-1 text-xs font-medium text-emerald-700">
        Entorno local
      </span>
    </header>
  );
}
