import { HcmDetailView } from "@/components/dh/views/HcmView";

export default async function HcmDetailPage({ params }: { params: Promise<{ id: string }> }) {
  const { id } = await params;
  return <HcmDetailView id={id} />;
}
