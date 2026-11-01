/* Reports: filter changes. MVC: reload the report partial with StayNest.ajax.loadPartial. */
(function () {
  "use strict";
  document.addEventListener("DOMContentLoaded", () => {
    document.querySelectorAll(".page-actions select").forEach((s) =>
      s.addEventListener("change", () => window.StayNest.toast("In the app, the report reloads for " + s.value + ".")));
  });
})();
