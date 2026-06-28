"use client";

import { useState } from "react";
import { Button } from "@/components/ui/button";
import { tiService } from "@/lib/services/tiService";
import {
  apisFilename,
  downloadTxt,
  entitiesFilename,
  formatApisTxt,
  formatEntitiesTxt,
  formatSchemaTxt,
  schemaFilename,
} from "@/lib/ti/tiExport";

type ExportKind = "schema" | "apis" | "entities";

const LABELS: Record<ExportKind, string> = {
  schema: "Descargar schema (.txt)",
  apis: "Descargar APIs (.txt)",
  entities: "Descargar entidades (.txt)",
};

export function TiExportButtons({
  kinds = ["schema", "apis", "entities"],
  variant = "secondary",
}: {
  kinds?: ExportKind[];
  variant?: "primary" | "secondary" | "ghost";
}) {
  const [loading, setLoading] = useState<ExportKind | null>(null);

  async function handleExport(kind: ExportKind) {
    setLoading(kind);
    try {
      const env = await tiService.getEnvironment().catch(() => null);

      if (kind === "schema") {
        const schema = await tiService.getSchema();
        downloadTxt(schemaFilename(), formatSchemaTxt(schema, env));
      } else if (kind === "apis") {
        const apis = await tiService.getApis();
        downloadTxt(apisFilename(), formatApisTxt(apis, env));
      } else {
        const entities = await tiService.listEntities();
        downloadTxt(entitiesFilename(), formatEntitiesTxt(entities, env));
      }
    } finally {
      setLoading(null);
    }
  }

  return (
    <div className="flex flex-wrap gap-2">
      {kinds.map((kind) => (
        <Button
          key={kind}
          type="button"
          variant={variant}
          disabled={loading !== null}
          onClick={() => handleExport(kind).catch(console.error)}
        >
          {loading === kind ? "Generando…" : LABELS[kind]}
        </Button>
      ))}
    </div>
  );
}
