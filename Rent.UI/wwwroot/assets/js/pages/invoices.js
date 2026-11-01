/* Record payment modal (BR-18): amount must be > 0 and not more than outstanding. */
(function () {
  "use strict";
  const SN = window.StayNest;
  document.addEventListener("DOMContentLoaded", () => {
    const modal = document.getElementById("payment-modal");
    if (!modal) return;
    const form = document.getElementById("payment-form");
    const amount = document.getElementById("pay-amount");
    const err = document.getElementById("pay-amount-error");
    let ctx = null;

    modal.addEventListener("modal:trigger", (e) => {
      const t = e.detail;
      ctx = { bill: t.dataset.bill, tenant: t.dataset.tenant, outstanding: Number(t.dataset.outstanding) };
      form.reset();
      form.querySelectorAll(".field").forEach((f) => f.classList.remove("invalid"));
      document.getElementById("payment-sub").textContent = `${ctx.bill} for ${ctx.tenant}`;
      document.getElementById("payment-outstanding").textContent = SN.formatINR(ctx.outstanding);
      amount.value = ctx.outstanding;
      amount.max = ctx.outstanding;
      const d = new Date();
      document.getElementById("pay-date").value = `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, "0")}-${String(d.getDate()).padStart(2, "0")}`;
    });

    form.addEventListener("submit", (e) => {
      e.preventDefault();
      if (!SN.validate(form)) return;
      const v = Number(amount.value);
      let msg = "";
      if (!(v > 0)) msg = "Enter an amount greater than 0.";
      else if (v > ctx.outstanding) msg = `Amount cannot be more than ${SN.formatINR(ctx.outstanding)}.`;
      amount.closest(".field").classList.toggle("invalid", !!msg);
      if (msg) { err.textContent = msg; return; }

      const row = document.querySelector(`[data-bill-row="${ctx.bill}"]`);
      const left = ctx.outstanding - v;
      if (row) {
        row.querySelector("[data-status]").innerHTML = left === 0 ? '<span class="pill pill-green">Paid</span>' : '<span class="pill pill-orange">Partially paid</span>';
        if (left === 0) row.querySelector("[data-action]").innerHTML = '<span class="text-muted">Settled</span>';
        else row.querySelector("[data-action] button").dataset.outstanding = String(left);
      }
      SN.closeModal(modal);
      SN.toast(left === 0 ? "Payment recorded. Bill is paid." : `Payment recorded. ${SN.formatINR(left)} still due.`);
    });
  });
})();
