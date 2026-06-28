import { notFound } from "next/navigation";
import { AppShell, Topbar } from "@/components/layout/AppShell";
import { modules } from "@/lib/modules";

type Props = {
  params: Promise<{ slug: string }>;
};

export function generateStaticParams() {
  return modules.map((module) => ({ slug: module.id }));
}

export default async function ModulePage({ params }: Props) {
  const { slug } = await params;
  const module = modules.find((item) => item.id === slug);

  if (!module) {
    notFound();
  }

  return (
    <AppShell>
      <Topbar />
      <main className="flex-1 overflow-y-auto bg-slate-50 p-6">
        <div className="mb-6 flex items-center gap-4">
          <div className={`h-12 w-12 rounded-2xl ${module.color}`} />
          <div>
            <h1 className="text-2xl font-bold text-slate-900">{module.name}</h1>
            <p className="text-slate-600">{module.description}</p>
          </div>
        </div>

        <section className="rounded-2xl border border-slate-200 bg-white p-6 shadow-sm">
          <h2 className="text-lg font-semibold text-slate-900">Entidades del módulo</h2>
          <p className="mt-1 text-sm text-slate-500">
            Modelo de datos MVP listo en SQL Server y expuesto vía API REST.
          </p>
          <ul className="mt-4 grid gap-2 sm:grid-cols-2 lg:grid-cols-3">
            {module.tables.map((table) => (
              <li
                key={table}
                className="rounded-lg border border-slate-100 bg-slate-50 px-4 py-3 text-sm text-slate-700"
              >
                {table}
              </li>
            ))}
          </ul>
        </section>

        <section className="mt-6 rounded-2xl border border-dashed border-slate-300 bg-white p-6">
          <h3 className="font-medium text-slate-900">Próximo paso</h3>
          <p className="mt-2 text-sm text-slate-600">
            Conecta este módulo con los endpoints en{" "}
            <code className="rounded bg-slate-100 px-1.5 py-0.5 text-xs">
              /api/{module.id}
            </code>{" "}
            para implementar listados, formularios y flujos de aprobación.
          </p>
        </section>
      </main>
    </AppShell>
  );
}
