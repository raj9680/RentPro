/* Property detail: collapsible space tree and add / edit space modal.
   Enforces the level rules from BR-03, BR-04 and BR-06 on the client (the API enforces them too). */
(function () {
  "use strict";
  const SN = window.StayNest;
  const ALLOWED = { Property: ["Floor", "Flat", "Room"], Floor: ["Flat", "Room"], Flat: ["Room"], Room: ["Bed"] };

  function childrenOf(id) { return Array.from(document.querySelectorAll(`.tree-row[data-parent="${id}"]`)); }
  function setVisible(row, visible) {
    childrenOf(row.dataset.id).forEach((child) => {
      child.classList.toggle("hidden-child", !visible);
      const childOpen = !child.classList.contains("collapsed");
      setVisible(child, visible && childOpen);
    });
  }

  document.addEventListener("DOMContentLoaded", () => {
    document.querySelectorAll(".tree-toggle").forEach((btn) => {
      btn.addEventListener("click", () => {
        const row = btn.closest(".tree-row");
        const collapsed = row.classList.toggle("collapsed");
        btn.setAttribute("aria-expanded", String(!collapsed));
        setVisible(row, !collapsed);
      });
    });
    const all = (open) => document.querySelectorAll(".tree-row[data-id]").forEach((r) => {
      r.classList.toggle("collapsed", !open);
      r.classList.toggle("hidden-child", !open && r.dataset.parent !== "");
      r.querySelector(".tree-toggle")?.setAttribute("aria-expanded", String(open));
    });
    document.getElementById("expand-all").addEventListener("click", () => all(true));
    document.getElementById("collapse-all").addEventListener("click", () => all(false));

    // Modal setup
    const modal = document.getElementById("space-modal");
    const form = document.getElementById("space-form");
    const type = document.getElementById("s-type");
    const cap = document.getElementById("s-cap");
    const rentable = document.getElementById("s-rentable");
    const note = document.getElementById("type-note");
    const title = document.getElementById("space-modal-title");
    const sub = document.getElementById("space-modal-sub");
    const submit = document.getElementById("space-submit");

    function applyType() {
      const t = type.value;
      const isFloor = t === "Floor", isBed = t === "Bed";
      rentable.disabled = isFloor || isBed;
      if (isFloor) rentable.checked = false;
      if (isBed) { rentable.checked = true; cap.value = 1; }
      cap.readOnly = isBed;
      document.querySelectorAll(".rent-field").forEach((f) => f.classList.toggle("hidden", !rentable.checked));
      note.classList.toggle("hidden", !(isFloor || isBed));
      note.querySelector("span").textContent = isFloor ? "Floors group spaces and are never rented." : isBed ? "A bed always holds exactly 1 person and is always rentable." : "";
    }
    type.addEventListener("change", applyType);
    rentable.addEventListener("change", applyType);

    modal.addEventListener("modal:trigger", (e) => {
      const trigger = e.detail;
      form.reset();
      document.querySelectorAll("#space-form .field").forEach((f) => f.classList.remove("invalid"));
      if (trigger.dataset.edit) {
        title.textContent = "Edit " + trigger.dataset.edit;
        sub.textContent = "Rent changes apply to new tenants only.";
        submit.textContent = "Save changes";
        type.innerHTML = `<option>${trigger.dataset.type}</option>`;
        type.disabled = true;
        document.getElementById("s-name").value = trigger.dataset.edit;
        cap.value = trigger.dataset.capacity;
      } else {
        const parentType = trigger.dataset.parentType;
        title.textContent = "Add space";
        sub.textContent = "Inside " + trigger.dataset.parent;
        submit.textContent = "Add space";
        type.disabled = false;
        type.innerHTML = (ALLOWED[parentType] || []).map((t) => `<option>${t}</option>`).join("");
      }
      applyType();
    });

    form.addEventListener("submit", (e) => {
      e.preventDefault();
      const capOk = Number(cap.value) >= 1;
      cap.closest(".field").classList.toggle("invalid", !capOk);
      if (!SN.validate(form) || !capOk) return;
      SN.closeModal(modal);
      SN.toast(submit.textContent === "Add space" ? "Space added. Capacity totals were updated." : "Changes saved.");
    });
  });
})();
