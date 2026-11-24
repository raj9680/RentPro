/* Move-in wizard (UI only): step navigation and add / remove co-occupant rows.
   No validation and no calculations. On the last step the form is submitted normally. */
(function () {
  "use strict";

  document.addEventListener("DOMContentLoaded", () => {
    const form = document.getElementById("move-in-form");
    if (!form) return;

    const steps = Array.from(form.querySelectorAll(".wizard-step"));
    const indicators = Array.from(document.querySelectorAll("[data-step-ind]"));
    const prev = document.getElementById("prev-step");
    const next = document.getElementById("next-step");
    const rows = document.getElementById("occupant-rows");
    const template = document.getElementById("occupant-template");
    const addBtn = document.getElementById("add-occupant");
    const nextLabel = next.innerHTML; // "Continue" + arrow icon
    let current = 0;

    function show(i) {
      current = i;
      const last = i === steps.length - 1;
      steps.forEach((s, n) => s.classList.toggle("active", n === i));
      indicators.forEach((s, n) => {
        s.classList.toggle("active", n === i);
        s.classList.toggle("done", n < i);
      });
      prev.disabled = i === 0;
      next.innerHTML = last ? "Confirm move-in" : nextLabel;
    }

    prev.addEventListener("click", () => show(Math.max(0, current - 1)));
    next.addEventListener("click", () => {
      if (current < steps.length - 1) {
        show(current + 1);
      } else {
        form.requestSubmit();
      }
    });

    // Repeatable co-occupant rows
    if (addBtn && rows && template) {
      addBtn.addEventListener("click", () => rows.appendChild(template.content.cloneNode(true)));
      rows.addEventListener("click", (e) => {
        const btn = e.target.closest(".remove-occupant");
        if (btn) btn.closest(".occupant-row").remove();
      });
    }

    show(0);
  });
})();
