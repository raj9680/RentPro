/* ==========================================================================
   StayNest common UI behavior
   Sidebar, nav groups, dropdowns, modals, tabs, toasts, password toggles,
   client-side table search and confirm dialogs.
   No dependencies. In the MVC app this file is loaded from _Layout.cshtml.
   ========================================================================== */
(function () {
  "use strict";

  const SN = (window.StayNest = window.StayNest || {});
  const $ = (sel, root = document) => root.querySelector(sel);
  const $$ = (sel, root = document) => Array.from(root.querySelectorAll(sel));
  SN.$ = $;
  SN.$$ = $$;

  /* ---------- Sidebar ---------- */
  function initSidebar() {
    const app = $(".app");
    if (!app) return;
    const toggle = $(".sidebar-toggle");
    const menuBtn = $(".menu-btn");
    const overlay = $(".sidebar-overlay");

    toggle && toggle.addEventListener("click", () => {
      app.classList.toggle("sidebar-collapsed");
      toggle.setAttribute("aria-expanded", String(!app.classList.contains("sidebar-collapsed")));
    });
    menuBtn && menuBtn.addEventListener("click", () => app.classList.add("sidebar-open"));
    overlay && overlay.addEventListener("click", () => app.classList.remove("sidebar-open"));

    $$(".nav-group-toggle").forEach((btn) => {
      btn.addEventListener("click", () => {
        if (app.classList.contains("sidebar-collapsed")) {
          app.classList.remove("sidebar-collapsed");
        }
        const group = btn.closest(".nav-group");
        const open = group.classList.toggle("open");
        btn.setAttribute("aria-expanded", String(open));
      });
    });
  }

  /* ---------- Dropdowns ---------- */
  function initDropdowns() {
    $$("[data-dropdown]").forEach((trigger) => {
      trigger.addEventListener("click", (e) => {
        e.stopPropagation();
        const dd = trigger.closest(".dropdown");
        const isOpen = dd.classList.contains("open");
        $$(".dropdown.open").forEach((d) => d.classList.remove("open"));
        if (!isOpen) dd.classList.add("open");
        trigger.setAttribute("aria-expanded", String(!isOpen));
      });
    });
    document.addEventListener("click", (e) => {
      if (!e.target.closest(".dropdown-menu")) $$(".dropdown.open").forEach((d) => d.classList.remove("open"));
    });
  }

  /* ---------- Modals ---------- */
  let lastFocus = null;
  SN.openModal = function (id) {
    const m = document.getElementById(id);
    if (!m) return;
    lastFocus = document.activeElement;
    m.classList.add("open");
    m.setAttribute("aria-hidden", "false");
    const first = m.querySelector("input:not([type=hidden]), select, textarea, button");
    first && setTimeout(() => first.focus(), 30);
    m.dispatchEvent(new CustomEvent("modal:open"));
  };
  SN.closeModal = function (id) {
    const m = typeof id === "string" ? document.getElementById(id) : id;
    if (!m) return;
    m.classList.remove("open");
    m.setAttribute("aria-hidden", "true");
    lastFocus && lastFocus.focus();
  };
  function initModals() {
    document.addEventListener("click", (e) => {
      const opener = e.target.closest("[data-modal-open]");
      if (opener) {
        e.preventDefault();
        SN.openModal(opener.getAttribute("data-modal-open"));
        document.getElementById(opener.getAttribute("data-modal-open"))
          ?.dispatchEvent(new CustomEvent("modal:trigger", { detail: opener }));
        return;
      }
      const closer = e.target.closest("[data-modal-close]");
      if (closer) {
        SN.closeModal(closer.closest(".modal"));
        return;
      }
      if (e.target.classList && e.target.classList.contains("modal")) SN.closeModal(e.target);
    });
    document.addEventListener("keydown", (e) => {
      if (e.key !== "Escape") return;
      $$(".modal.open").forEach((m) => SN.closeModal(m));
      $$(".dropdown.open").forEach((d) => d.classList.remove("open"));
      $(".app")?.classList.remove("sidebar-open");
    });
  }

  /* ---------- Confirm dialog ---------- */
  SN.confirm = function ({ title, message, confirmText = "Confirm", danger = false }) {
    return new Promise((resolve) => {
      let m = document.getElementById("confirm-modal");
      if (!m) {
        m = document.createElement("div");
        m.className = "modal";
        m.id = "confirm-modal";
        m.setAttribute("role", "dialog");
        m.setAttribute("aria-modal", "true");
        m.innerHTML =
          '<div class="modal-dialog" style="max-width:420px"><div class="modal-head"><div><h2 data-c-title></h2><p data-c-msg></p></div></div>' +
          '<div class="modal-foot" style="padding-top:20px"><button type="button" class="btn btn-light" data-c-cancel>Cancel</button><button type="button" class="btn" data-c-ok></button></div></div>';
        document.body.appendChild(m);
      }
      m.querySelector("[data-c-title]").textContent = title;
      m.querySelector("[data-c-msg]").textContent = message;
      const ok = m.querySelector("[data-c-ok]");
      ok.textContent = confirmText;
      ok.className = "btn " + (danger ? "btn-danger" : "btn-primary");
      const done = (v) => { SN.closeModal(m); resolve(v); };
      ok.onclick = () => done(true);
      m.querySelector("[data-c-cancel]").onclick = () => done(false);
      SN.openModal("confirm-modal");
    });
  };
  function initConfirmLinks() {
    document.addEventListener("click", async (e) => {
      const el = e.target.closest("[data-confirm]");
      if (!el) return;
      e.preventDefault();
      const ok = await SN.confirm({
        title: el.getAttribute("data-confirm"),
        message: el.getAttribute("data-confirm-message") || "",
        confirmText: el.getAttribute("data-confirm-text") || "Confirm",
        danger: el.hasAttribute("data-confirm-danger"),
      });
      if (!ok) return;
      if (el.tagName === "A" && el.getAttribute("href") && el.getAttribute("href") !== "#") {
        window.location.href = el.getAttribute("href");
      } else {
        el.dispatchEvent(new CustomEvent("confirmed", { bubbles: true }));
      }
    });
  }

  /* ---------- Tabs ---------- */
  function initTabs() {
    $$("[data-tabs]").forEach((tabs) => {
      const buttons = $$(".tab", tabs);
      buttons.forEach((btn) => {
        btn.addEventListener("click", () => {
          buttons.forEach((b) => { b.classList.remove("active"); b.setAttribute("aria-selected", "false"); });
          btn.classList.add("active");
          btn.setAttribute("aria-selected", "true");
          const target = btn.getAttribute("data-tab");
          $$("[data-tab-panel]").forEach((p) => p.classList.toggle("active", p.getAttribute("data-tab-panel") === target));
        });
      });
    });
  }

  /* ---------- Toasts ---------- */
  SN.toast = function (message, type = "success") {
    let stack = $(".toast-stack");
    if (!stack) {
      stack = document.createElement("div");
      stack.className = "toast-stack";
      stack.setAttribute("aria-live", "polite");
      document.body.appendChild(stack);
    }
    const t = document.createElement("div");
    t.className = "toast" + (type === "error" ? " error" : "");
    t.textContent = message;
    stack.appendChild(t);
    setTimeout(() => t.remove(), 3500);
  };

  /* ---------- Password visibility ---------- */
  function initPasswordToggles() {
    $$("[data-password-toggle]").forEach((btn) => {
      btn.addEventListener("click", () => {
        const input = document.getElementById(btn.getAttribute("data-password-toggle"));
        const show = input.type === "password";
        input.type = show ? "text" : "password";
        btn.setAttribute("aria-label", show ? "Hide password" : "Show password");
        btn.querySelector(".eye-on")?.classList.toggle("hidden", show);
        btn.querySelector(".eye-off")?.classList.toggle("hidden", !show);
      });
    });
  }

  /* ---------- Client-side table search (template preview only) ---------- */
  function initTableSearch() {
    $$("[data-table-search]").forEach((input) => {
      const table = document.querySelector(input.getAttribute("data-table-search"));
      if (!table) return;
      input.addEventListener("input", () => {
        const q = input.value.trim().toLowerCase();
        $$("tbody tr", table).forEach((tr) => {
          tr.style.display = !q || tr.textContent.toLowerCase().includes(q) ? "" : "none";
        });
      });
    });
  }

  /* ---------- Simple form validation helper ---------- */
  SN.validate = function (form) {
    let valid = true;
    $$("[required], [data-rule]", form).forEach((input) => {
      const field = input.closest(".field");
      if (!field) return;
      let ok = input.value.trim() !== "";
      const rule = input.getAttribute("data-rule");
      if (ok && rule === "email") ok = /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(input.value.trim());
      if (ok && rule === "phone") ok = /^[6-9]\d{9}$/.test(input.value.replace(/\s/g, ""));
      if (ok && rule === "pin") ok = /^\d{6}$/.test(input.value.trim());
      if (ok && rule === "money") ok = Number(input.value) >= 0;
      if (!input.hasAttribute("required") && input.value.trim() === "") ok = true;
      field.classList.toggle("invalid", !ok);
      if (!ok) valid = false;
    });
    const firstInvalid = form.querySelector(".field.invalid input, .field.invalid select, .field.invalid textarea");
    firstInvalid && firstInvalid.focus();
    return valid;
  };

  /* ---------- Formatting ---------- */
  SN.formatINR = function (n) {
    return "\u20B9" + Number(n).toLocaleString("en-IN", { maximumFractionDigits: 0 });
  };

  document.addEventListener("DOMContentLoaded", () => {
    initSidebar();
    initDropdowns();
    initModals();
    initConfirmLinks();
    initTabs();
    initPasswordToggles();
    initTableSearch();
  });
})();
