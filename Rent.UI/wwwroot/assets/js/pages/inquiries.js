/* Inquiry status changes. MVC: PUT /api/v1/inquiries/{id}/status via the Web controller. */
(function () {
  "use strict";
  const SN = window.StayNest;
  const PILL = { Contacted: "pill-green", Closed: "pill-grey" };
  document.addEventListener("click", (e) => {
    const btn = e.target.closest("[data-inquiry-status]");
    if (!btn) return;
    const s = btn.dataset.inquiryStatus;
    const row = btn.closest("tr");
    row.querySelector("[data-status]").innerHTML = `<span class="pill ${PILL[s]}">${s}</span>`;
    btn.closest(".dropdown").classList.remove("open");
    SN.toast(s === "Closed" ? "Inquiry closed." : "Marked as contacted.");
  });
})();
