/* Space status grid: property switcher and details panel.
   MVC: tiles come from GET /api/v1/properties/{id}/space-status; panel can load a partial view via StayNest.ajax.loadPartial. */
(function () {
  "use strict";
  const esc = (s) => String(s || "").replace(/[&<>"]/g, (c) => ({ "&": "&amp;", "<": "&lt;", ">": "&gt;", '"': "&quot;" }[c]));
  const PILL = { Vacant: "pill-green", Occupied: "pill-red", "On notice": "pill-orange", Blocked: "pill-grey" };

  function render(tile) {
    const d = tile.dataset;
    let rows = `<dt>Type</dt><dd>${esc(d.type)}</dd><dt>Location</dt><dd>${esc(d.path)}</dd><dt>Capacity</dt><dd>${esc(d.capacity)} ${d.capacity === "1" ? "person" : "people"}</dd><dt>Monthly rent</dt><dd>${esc(d.rent)}</dd>`;
    if (d.tenant) rows += `<dt>Staying</dt><dd>${esc(d.tenant)}</dd><dt>Since</dt><dd>${esc(d.since)}</dd>`;
    let action = "";
    if (d.status === "Vacant") action = `<a class="btn btn-primary btn-block" href="move-in.html">Move in a tenant</a>`;
    else if (d.status === "Blocked") action = `<div class="alert alert-warning"><span>${esc(d.reason || "A space above or below this one is rented.")}</span></div>`;
    else action = `<a class="btn btn-light btn-block" href="tenancy-detail.html">View tenant</a>`;
    return `<div class="card-head" style="margin-bottom:12px"><h2 class="card-title">${esc(d.name)}</h2><span class="pill ${PILL[d.status]}">${esc(d.status)}</span></div>
      <dl class="dl" style="grid-template-columns:100px 1fr;font-size:13px;margin-bottom:18px">${rows}</dl>${action}`;
  }

  document.addEventListener("DOMContentLoaded", () => {
    const panel = document.getElementById("space-panel");
    document.querySelectorAll(".tile").forEach((tile) => {
      tile.addEventListener("click", () => {
        document.querySelectorAll(".tile.selected").forEach((t) => t.classList.remove("selected"));
        tile.classList.add("selected");
        panel.innerHTML = render(tile);
      });
    });
    const sw = document.getElementById("property-switch");
    sw.addEventListener("change", () => {
      document.querySelectorAll("[data-property]").forEach((s) => s.classList.toggle("hidden", s.dataset.property !== sw.value));
      panel.innerHTML = '<div class="panel-empty"><p>Select a space to see its details.</p></div>';
    });
  });
})();
