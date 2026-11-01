/* Move-in wizard: step navigation, capacity-limited co-occupants (BR-08),
   move-in date window (BR-11) and first-bill proration preview (BR-16, BR-17). */
(function () {
  "use strict";
  const SN = window.StayNest;
  const MONTHS = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
  const fmtDate = (d) => `${String(d.getDate()).padStart(2, "0")} ${MONTHS[d.getMonth()]} ${d.getFullYear()}`;
  const iso = (d) => `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, "0")}-${String(d.getDate()).padStart(2, "0")}`;

  // Same rule as the server: rent x days stayed / days in month, rounded to the nearest rupee
  function prorate(rent, start) {
    const dim = new Date(start.getFullYear(), start.getMonth() + 1, 0).getDate();
    const days = dim - start.getDate() + 1;
    return { days, dim, amount: Math.round((rent * days) / dim) };
  }

  document.addEventListener("DOMContentLoaded", () => {
    const form = document.getElementById("move-in-form");
    const steps = Array.from(form.querySelectorAll(".wizard-step"));
    const inds = Array.from(document.querySelectorAll("[data-step-ind]"));
    const prev = document.getElementById("prev-step");
    const next = document.getElementById("next-step");
    const rows = document.getElementById("occupant-rows");
    const tpl = document.getElementById("occupant-template");
    const addBtn = document.getElementById("add-occupant");
    const limit = document.getElementById("occupant-limit");
    const capTag = document.getElementById("capacity-tag");
    const date = document.getElementById("m-date");
    const rent = document.getElementById("m-rent");
    const dep = document.getElementById("m-dep");
    let current = 0;

    const today = new Date(); today.setHours(0, 0, 0, 0);
    const minDate = new Date(today); minDate.setDate(minDate.getDate() - 30);
    date.max = iso(today); date.min = iso(minDate); date.value = iso(today);

    const selected = () => form.querySelector('input[name="SpaceId"]:checked');
    const capacity = () => Number(selected().dataset.capacity);

    function refreshOccupants() {
      const used = 1 + rows.children.length;
      capTag.lastChild.textContent = `${used} of ${capacity()} places used`;
      const full = used >= capacity();
      addBtn.disabled = full;
      limit.textContent = full ? (capacity() === 1 ? "This space holds 1 person, so co-occupants cannot be added." : "This space is full.") : "";
    }
    addBtn.addEventListener("click", () => {
      if (1 + rows.children.length >= capacity()) return;
      rows.appendChild(tpl.content.cloneNode(true));
      refreshOccupants();
    });
    rows.addEventListener("click", (e) => {
      const btn = e.target.closest(".remove-occupant");
      if (!btn) return;
      btn.closest(".occupant-row").remove();
      refreshOccupants();
    });
    form.querySelectorAll('input[name="SpaceId"]').forEach((r) => r.addEventListener("change", () => {
      while (1 + rows.children.length > capacity()) rows.lastElementChild.remove();
      rent.value = r.dataset.rent; dep.value = r.dataset.deposit;
      refreshOccupants();
    }));
    rent.value = selected().dataset.rent; dep.value = selected().dataset.deposit;

    document.getElementById("lookup-tenant").addEventListener("click", () => {
      const email = document.getElementById("t-email").value.trim().toLowerCase();
      const status = document.getElementById("lookup-status");
      if (email === "karan.arora@example.com") {
        document.getElementById("t-name").value = "Karan Arora";
        document.getElementById("t-phone").value = "9810052617";
        status.textContent = "Existing tenant found. Their account will be reused.";
      } else {
        status.textContent = "No existing account. A new tenant account will be created.";
      }
    });

    function summary() {
      const s = selected();
      const start = new Date(date.value + "T00:00:00");
      const p = prorate(Number(rent.value || 0), start);
      const due = new Date(start); due.setDate(due.getDate() + 5);
      const people = 1 + rows.children.length;
      document.getElementById("summary").innerHTML =
        `<dt>Space</dt><dd>${s.dataset.name}</dd>` +
        `<dt>Primary tenant</dt><dd>${document.getElementById("t-name").value || "-"}</dd>` +
        `<dt>People</dt><dd>${people}</dd>` +
        `<dt>First bill</dt><dd>${SN.formatINR(p.amount)} for ${p.days} of ${p.dim} days, due ${fmtDate(due)}</dd>` +
        `<dt>From next month</dt><dd>${SN.formatINR(rent.value || 0)} on the 1st, due on the 5th</dd>`;
    }
    [date, rent].forEach((i) => i.addEventListener("input", summary));

    function validDate() {
      const d = new Date(date.value + "T00:00:00");
      const ok = date.value && d <= today && d >= minDate;
      date.closest(".field").classList.toggle("invalid", !ok);
      return ok;
    }

    function show(i) {
      current = i;
      steps.forEach((s, n) => s.classList.toggle("active", n === i));
      inds.forEach((s, n) => { s.classList.toggle("active", n === i); s.classList.toggle("done", n < i); });
      prev.disabled = i === 0;
      next.innerHTML = i === steps.length - 1 ? "Confirm move-in" : 'Continue<svg class="icon" viewBox="0 0 24 24" aria-hidden="true"><path d="m9 6 6 6-6 6"/></svg>';
      if (i === 2) refreshOccupants();
      if (i === 3) summary();
    }
    prev.addEventListener("click", () => show(Math.max(0, current - 1)));
    next.addEventListener("click", () => {
      const step = steps[current];
      if (current > 0 && !SN.validate(step)) return;
      if (current === 3 && !validDate()) return;
      if (current < steps.length - 1) return show(current + 1);
      SN.toast("Tenant moved in. Welcome email queued and first bill created.");
      setTimeout(() => (window.location.href = "tenancy-detail.html"), 1000);
    });
    show(0);
  });
})();
