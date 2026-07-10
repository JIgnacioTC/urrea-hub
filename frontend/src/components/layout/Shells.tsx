"use client";

import { usePathname, useRouter } from "next/navigation";
import { useEffect, useMemo, useState } from "react";
import { AppLayout } from "@/components/layout/AppLayout";
import { portalService } from "@/lib/services/portalService";
import { getSession, isRhAdmin, isTiAdmin, type Session } from "@/lib/auth";
import {
  ADMIN_DH_SECTIONS,
  ADMIN_TI_SECTIONS,
  buildMobileNav,
  buildPortalSections,
  filterNavSections,
  flattenRhLinks,
  flattenTiLinks,
  RH_SECTIONS,
  TI_SECTIONS,
  type PortalNavSection,
} from "@/lib/portal/navigation";
import type { ColaboradorPerfil } from "@/lib/types";

function LoadingScreen() {
  return (
    <div className="flex h-[100dvh] items-center justify-center bg-slate-50">
      <div className="h-8 w-8 animate-pulse rounded-full bg-urrea-primary/20" />
    </div>
  );
}

function PortalShellInner({ children }: { children: React.ReactNode }) {
  const pathname = usePathname();
  const router = useRouter();
  const [session, setSession] = useState<Session | null>(null);
  const [perfil, setPerfil] = useState<ColaboradorPerfil | null>(null);

  useEffect(() => {
    const s = getSession();
    if (!s) {
      router.replace("/login");
      return;
    }
    setSession(s);
    portalService.getMe().then(setPerfil).catch(console.error);
  }, [router]);

  const sections = useMemo(() => (session ? buildPortalSections(session) : []), [session]);
  const mobileNavLinks = useMemo(() => (session ? buildMobileNav(session) : []), [session]);

  if (!session) return <LoadingScreen />;

  return (
    <AppLayout
      session={session}
      perfil={perfil}
      sections={sections}
      mobileNavLinks={mobileNavLinks}
      initialWorkspace="portal"
    >
      {children}
    </AppLayout>
  );
}

function RhShellInner({
  children,
  sectionsOverride,
}: {
  children: React.ReactNode;
  sectionsOverride?: PortalNavSection[];
}) {
  const pathname = usePathname();
  const router = useRouter();
  const [session, setSession] = useState<Session | null>(null);
  const [perfil, setPerfil] = useState<ColaboradorPerfil | null>(null);

  useEffect(() => {
    const s = getSession();
    if (!s) {
      router.replace("/login");
      return;
    }
    if (!isRhAdmin(s)) {
      router.replace("/portal");
      return;
    }
    setSession(s);
    portalService.getMe().then(setPerfil).catch(console.error);
  }, [router]);

  const sections = useMemo(
    () => (session ? filterNavSections(sectionsOverride ?? RH_SECTIONS, session) : []),
    [session, sectionsOverride],
  );

  if (!session) return <LoadingScreen />;

  return (
    <AppLayout
      session={session}
      perfil={perfil}
      sections={sections}
      mobileNavLinks={flattenRhLinks(sections)}
      initialWorkspace="rh"
    >
      {children}
    </AppLayout>
  );
}

function TiShellInner({
  children,
  sectionsOverride,
}: {
  children: React.ReactNode;
  sectionsOverride?: PortalNavSection[];
}) {
  const pathname = usePathname();
  const router = useRouter();
  const [session, setSession] = useState<Session | null>(null);
  const [perfil, setPerfil] = useState<ColaboradorPerfil | null>(null);

  useEffect(() => {
    const s = getSession();
    if (!s) {
      router.replace("/login");
      return;
    }
    if (!isTiAdmin(s)) {
      router.replace("/portal");
      return;
    }
    setSession(s);
    portalService.getMe().then(setPerfil).catch(console.error);
  }, [router]);

  const sections = useMemo(
    () => (session ? filterNavSections(sectionsOverride ?? TI_SECTIONS, session) : []),
    [session, sectionsOverride],
  );

  if (!session) return <LoadingScreen />;

  return (
    <AppLayout
      session={session}
      perfil={perfil}
      sections={sections}
      mobileNavLinks={flattenTiLinks(sections)}
      initialWorkspace="ti"
    >
      {children}
    </AppLayout>
  );
}

export function PortalShell({ children }: { children: React.ReactNode }) {
  return <PortalShellInner>{children}</PortalShellInner>;
}

export function RhShell({ children }: { children: React.ReactNode }) {
  return <RhShellInner>{children}</RhShellInner>;
}

export function TiShell({ children }: { children: React.ReactNode }) {
  return <TiShellInner>{children}</TiShellInner>;
}

export function AdminTiShell({ children }: { children: React.ReactNode }) {
  return <TiShellInner sectionsOverride={ADMIN_TI_SECTIONS}>{children}</TiShellInner>;
}

export function AdminDhShell({ children }: { children: React.ReactNode }) {
  return <RhShellInner sectionsOverride={ADMIN_DH_SECTIONS}>{children}</RhShellInner>;
}
