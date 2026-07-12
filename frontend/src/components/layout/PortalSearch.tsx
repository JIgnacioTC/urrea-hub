"use client";

import { useEffect, useMemo, useRef, useState } from "react";
import { useRouter } from "next/navigation";
import { DhIcon } from "@/components/dh/shared/icons";
import { PORTAL_HOME, type PortalNavLink, type PortalNavSection } from "@/lib/portal/navigation";
import { cn } from "@/lib/utils";

type SearchEntry = {
  link: PortalNavLink;
  sectionTitle: string;
  searchText: string;
};

function normalize(value: string) {
  return value
    .normalize("NFD")
    .replace(/[̀-ͯ]/g, "")
    .toLowerCase()
    .trim();
}

function collectEntries(link: PortalNavLink, sectionTitle: string, out: SearchEntry[]) {
  out.push({ link, sectionTitle, searchText: normalize(`${link.label} ${sectionTitle}`) });
  link.children?.forEach((child) => collectEntries(child, sectionTitle, out));
}

export function PortalSearch({
  sections,
  placeholder = "Buscar en el portal…",
  onNavigate,
}: {
  sections: PortalNavSection[];
  placeholder?: string;
  onNavigate?: () => void;
}) {
  const router = useRouter();
  const containerRef = useRef<HTMLDivElement>(null);
  const [query, setQuery] = useState("");
  const [open, setOpen] = useState(false);
  const [activeIndex, setActiveIndex] = useState(0);

  const entries = useMemo(() => {
    const out: SearchEntry[] = [];
    collectEntries(PORTAL_HOME, "Inicio", out);
    sections.forEach((section) => section.links.forEach((link) => collectEntries(link, section.title, out)));

    const seen = new Set<string>();
    return out.filter((entry) => (seen.has(entry.link.href) ? false : (seen.add(entry.link.href), true)));
  }, [sections]);

  const results = useMemo(() => {
    const q = normalize(query);
    if (!q) return [];
    return entries.filter((entry) => entry.searchText.includes(q)).slice(0, 8);
  }, [entries, query]);

  useEffect(() => {
    if (!open) return;
    function onClickOutside(e: MouseEvent) {
      if (containerRef.current && !containerRef.current.contains(e.target as Node)) setOpen(false);
    }
    document.addEventListener("mousedown", onClickOutside);
    return () => document.removeEventListener("mousedown", onClickOutside);
  }, [open]);

  const goTo = (href: string) => {
    router.push(href);
    setQuery("");
    setOpen(false);
    onNavigate?.();
  };

  const handleKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
    if (!open || results.length === 0) return;
    if (e.key === "ArrowDown") {
      e.preventDefault();
      setActiveIndex((i) => (i + 1) % results.length);
    } else if (e.key === "ArrowUp") {
      e.preventDefault();
      setActiveIndex((i) => (i - 1 + results.length) % results.length);
    } else if (e.key === "Enter") {
      e.preventDefault();
      goTo(results[activeIndex].link.href);
    } else if (e.key === "Escape") {
      setOpen(false);
    }
  };

  return (
    <div ref={containerRef} className="relative">
      <div className="relative">
        <DhIcon
          name="search"
          className="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-urrea-text-muted"
        />
        <input
          type="search"
          value={query}
          onChange={(e) => {
            setQuery(e.target.value);
            setActiveIndex(0);
            setOpen(true);
          }}
          onFocus={() => query && setOpen(true)}
          onKeyDown={handleKeyDown}
          placeholder={placeholder}
          className="h-9 w-full rounded-lg border border-urrea-border bg-urrea-bg py-2 pl-9 pr-3 text-sm outline-none focus:border-urrea-primary focus:ring-1 focus:ring-urrea-primary/30"
        />
      </div>

      {open && query && (
        <div className="absolute left-0 right-0 top-full z-30 mt-1.5 max-h-80 overflow-y-auto rounded-xl border border-urrea-border bg-white py-1.5 shadow-soft-lg">
          {results.length === 0 ? (
            <p className="px-3 py-3 text-sm text-urrea-text-muted">Sin resultados para &ldquo;{query}&rdquo;.</p>
          ) : (
            results.map((entry, index) => (
              <button
                key={entry.link.href}
                type="button"
                onClick={() => goTo(entry.link.href)}
                onMouseEnter={() => setActiveIndex(index)}
                className={cn(
                  "flex w-full items-center gap-3 px-3 py-2 text-left text-sm transition",
                  index === activeIndex ? "bg-urrea-primary/8 text-urrea-primary" : "text-urrea-text hover:bg-urrea-bg-soft"
                )}
              >
                <DhIcon
                  name={entry.link.icon}
                  className={cn("h-4 w-4 shrink-0", index === activeIndex ? "text-urrea-primary" : "text-urrea-text-muted")}
                />
                <span className="min-w-0 flex-1 truncate">{entry.link.label}</span>
                <span className="shrink-0 text-xs text-urrea-text-muted">{entry.sectionTitle}</span>
              </button>
            ))
          )}
        </div>
      )}
    </div>
  );
}
