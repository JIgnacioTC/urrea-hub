import { PortalHcmDetailView } from "@/components/portal/hcm/PortalHcmViews";

export default async function PortalHcmPersonaPage({ params }: { params: Promise<{ id: string }> }) {
  const { id } = await params;
  return <PortalHcmDetailView id={id} />;
}
