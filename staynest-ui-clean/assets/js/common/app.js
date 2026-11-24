/* ==========================================================================
   StayNest common UI behavior (design and layout only)
   Sidebar, nav groups, dropdowns, modals, confirm dialog, tabs and
   password visibility. No data handling and no validation.
   No dependencies. In the MVC app this file is loaded from _Layout.cshtml.
   ========================================================================== */
(function () {
  "use strict";

  const SN = (window.StayNest = window.StayNest || {});
  const $ = (sel, root = document) => root.querySelector(sel);
  const $$ = (sel, root = document) => Array.from(root.querySelectorAll(sel));

  /* ---------- Sidebar: collapse (desktop), open/close (mobile), nav groups ---------- */
  function initSidebar() {
    const app = $(".app");
    if (!app) return;
    const toggle = $(".sidebar-toggle");
    const menuBtn = $(".menu-btn");
    const overlay = $(".sidebar-overlay");

    if (toggle) {
      toggle.addEventListener("click", () => {
        app.classList.toggle("sidebar-collapsed");
        toggle.setAttribute("aria-expanded", String(!app.classList.contains("sidebar-collapsed")));
      });
    }
    if (menuBtn) menuBtn.addEventListener("click", () => app.classList.add("sidebar-open"));
    if (overlay) overlay.addEventListener("click", () => app.classList.remove("sidebar-open"));

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

  /* ---------- Modals: data-modal-open="id" / data-modal-close ---------- */
  let lastFocus = null;
  SN.openModal = function (id) {
    const m = document.getElementById(id);
    if (!m) return;
    lastFocus = document.activeElement;
    m.classList.add("open");
    m.setAttribute("aria-hidden", "false");
    const first = m.querySelector("input:not([type=hidden]), select, textarea, button");
    if (first) setTimeout(() => first.focus(), 30);
  };
  SN.closeModal = function (id) {
    const m = typeof id === "string" ? document.getElementById(id) : id;
    if (!m) return;
    m.classList.remove("open");
    m.setAttribute("aria-hidden", "true");
    if (lastFocus) lastFocus.focus();
  };
  function initModals() {
    document.addEventListener("click", (e) => {
      const opener = e.target.closest("[data-modal-open]");
      if (opener) {
        e.preventDefault();
        SN.openModal(opener.getAttribute("data-modal-open"));
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
      const app = $(".app");
      if (app) app.classList.remove("sidebar-open");
    });
  }

  /* ---------- Confirm dialog: data-confirm="Title" on a link or button ----------
     After "Confirm": a link navigates to its href; a button inside a form submits that form. */
  function confirmDialog(title, message, confirmText, danger) {
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
  }
  function initConfirm() {
    document.addEventListener("click", async (e) => {
      const el = e.target.closest("[data-confirm]");
      if (!el) return;
      e.preventDefault();
      const ok = await confirmDialog(
        el.getAttribute("data-confirm"),
        el.getAttribute("data-confirm-message") || "",
        el.getAttribute("data-confirm-text") || "Confirm",
        el.hasAttribute("data-confirm-danger")
      );
      if (!ok) return;
      if (el.tagName === "A" && el.getAttribute("href") && el.getAttribute("href") !== "#") {
        window.location.href = el.getAttribute("href");
      } else if (el.form) {
        el.form.requestSubmit(el.type === "submit" ? el : undefined);
      }
    });
  }

  /* ---------- Tabs: data-tabs > .tab[data-tab] + [data-tab-panel] ---------- */
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

  /* ---------- Password visibility: data-password-toggle="inputId" ---------- */
  function initPasswordToggles() {
    $$("[data-password-toggle]").forEach((btn) => {
      btn.addEventListener("click", () => {
        const input = document.getElementById(btn.getAttribute("data-password-toggle"));
        if (!input) return;
        const show = input.type === "password";
        input.type = show ? "text" : "password";
        btn.setAttribute("aria-label", show ? "Hide password" : "Show password");
        const on = btn.querySelector(".eye-on");
        const off = btn.querySelector(".eye-off");
        if (on) on.classList.toggle("hidden", show);
        if (off) off.classList.toggle("hidden", !show);
      });
    });
  }

  document.addEventListener("DOMContentLoaded", () => {
    initSidebar();
    initDropdowns();
    initModals();
    initConfirm();
    initTabs();
    initPasswordToggles();
  });
})();
