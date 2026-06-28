export function getInitials(nombre: string, apellidoPaterno: string, apellidoMaterno?: string) {
  const parts = [nombre, apellidoPaterno, apellidoMaterno].filter((p): p is string => Boolean(p));
  return parts
    .slice(0, 2)
    .map((p) => p.charAt(0).toUpperCase())
    .join("");
}

export function initialsFromFullName(fullName: string) {
  const parts = fullName.trim().split(/\s+/).filter(Boolean);
  if (parts.length >= 2) return `${parts[0][0]}${parts[1][0]}`.toUpperCase();
  return (parts[0]?.[0] ?? "?").toUpperCase();
}

export function getTenureLabel(fechaIngreso: string) {
  const start = new Date(fechaIngreso);
  const now = new Date();
  const months =
    (now.getFullYear() - start.getFullYear()) * 12 + (now.getMonth() - start.getMonth());
  if (months < 12) return `${Math.max(1, months)} mes${months === 1 ? "" : "es"}`;
  const years = Math.floor(months / 12);
  const rest = months % 12;
  if (rest === 0) return `${years} año${years === 1 ? "" : "s"}`;
  return `${years}a ${rest}m`;
}

export function avatarGradient(seed: string) {
  const hues = [
    "from-urrea-primary to-urrea-secondary",
    "from-urrea-secondary to-teal-600",
    "from-indigo-700 to-urrea-secondary",
    "from-slate-700 to-urrea-primary",
  ];
  let hash = 0;
  for (let i = 0; i < seed.length; i++) hash = seed.charCodeAt(i) + ((hash << 5) - hash);
  return hues[Math.abs(hash) % hues.length];
}
