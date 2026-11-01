/* Add / edit property: PIN code lookup with graceful fallback.
   MVC: GET /Properties/PinCode/{pin} (Web) -> GET /api/v1/lookups/pincode/{pin} (API, cached, resilient). */
(function () {
  "use strict";
  const SN = window.StayNest;
  // Sample lookups for the template preview only
  const SAMPLE = { "122001": ["Gurugram", "Haryana"], "201301": ["Noida", "Uttar Pradesh"], "560034": ["Bengaluru", "Karnataka"], "302001": ["Jaipur", "Rajasthan"] };

  document.addEventListener("DOMContentLoaded", () => {
    const form = document.getElementById("property-form");
    const pin = document.getElementById("p-pin");
    const status = document.getElementById("pin-status");
    const city = document.getElementById("p-city");
    const state = document.getElementById("p-state");

    pin.addEventListener("input", () => {
      pin.value = pin.value.replace(/\D/g, "").slice(0, 6);
      if (pin.value.length !== 6) { status.textContent = "City and state fill in automatically."; return; }
      status.textContent = "Looking up PIN code...";
      setTimeout(() => {
        const hit = SAMPLE[pin.value];
        if (hit) {
          city.value = hit[0];
          state.value = hit[1];
          status.textContent = "Filled from PIN code. You can still edit it.";
        } else {
          status.textContent = "We could not find this PIN code. Type the city and state.";
          city.focus();
        }
      }, 400);
    });

    form.addEventListener("submit", (e) => {
      e.preventDefault();
      if (!SN.validate(form)) return;
      SN.toast("Property saved.");
      setTimeout(() => (window.location.href = "property-detail.html"), 800);
    });
  });
})();
