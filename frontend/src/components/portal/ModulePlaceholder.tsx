import { ButtonLink } from "@/components/ui/button";
import { Card } from "@/components/ui/card";
import { PageContainer, PageHeader } from "@/components/ui/page-header";

export function ModulePlaceholder({
  title,
  subtitle,
  icon,
  description,
}: {
  title: string;
  subtitle?: string;
  icon: string;
  description: string;
}) {
  return (
    <PageContainer className="animate-fade-up">
      <PageHeader title={title} subtitle={subtitle} />
      <Card>
        <div className="flex flex-col items-center py-10 text-center">
          <span className="text-5xl" aria-hidden>{icon}</span>
          <p className="mt-4 max-w-md text-sm text-urrea-text-muted">{description}</p>
          <p className="mt-2 text-xs font-medium uppercase tracking-wide text-urrea-secondary">
            Próximamente en URREA Hub
          </p>
          <ButtonLink href="/portal" variant="secondary" className="mt-6">
            Volver al portal
          </ButtonLink>
        </div>
      </Card>
    </PageContainer>
  );
}
