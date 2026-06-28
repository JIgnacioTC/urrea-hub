import { redirect } from "next/navigation";

export default function NuevaSolicitudPage() {
  redirect("/portal/vacaciones?solicitar=1");
}
