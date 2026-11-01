/* Public vacancy search: client-side filtering for the template preview.
   MVC: GET /Search?city=&type=&minRent=&maxRent=&people=&amenities=&sort=&page= returns the results partial (cached, rate limited). */
(function () {
  "use strict";
  const SN = window.StayNest;
  document.addEventListener("DOMContentLoaded", () => {
    const form = document.getElementById("search-filters");
    const results = document.getElementById("results");
    const cards = Array.from(results.querySelectorAll(".listing"));
    const count = document.getElementById("result-count");
    const empty = document.getElementById("no-results");
    const sort = document.getElementById("f-sort");

    function apply() {
      const city = document.getElementById("f-city").value;
      const type = form.querySelector('input[name="type"]:checked')?.value || "";
      const min = Number(document.getElementById("f-min").value || 0);
      const max = Number(document.getElementById("f-max").value || Infinity);
      const people = Number(document.getElementById("f-people").value);
      const amen = Array.from(form.querySelectorAll('.chip input[type="checkbox"]:checked')).map((c) => c.value);
      let shown = 0;
      cards.forEach((c) => {
        const d = c.dataset;
        const tags = c.textContent;
        const ok = (!city || d.city === city) && (!type || d.type === type) && Number(d.rent) >= min && Number(d.rent) <= max &&
          Number(d.capacity) >= people && amen.every((a) => tags.includes(a));
        c.classList.toggle("hidden", !ok);
        if (ok) shown++;
      });
      const key = sort.value;
      cards.slice().sort((a, b) => key === "rent-desc" ? b.dataset.rent - a.dataset.rent : key === "capacity" ? b.dataset.capacity - a.dataset.capacity : a.dataset.rent - b.dataset.rent)
        .forEach((c) => results.appendChild(c));
      count.textContent = `${shown} ${shown === 1 ? "space" : "spaces"} found`;
      empty.classList.toggle("hidden", shown > 0);
    }
    form.addEventListener("input", apply);
    form.addEventListener("change", apply);
    form.addEventListener("reset", () => setTimeout(apply, 0));
    sort.addEventListener("change", apply);
    apply();

    const modal = document.getElementById("inquiry-modal");
    modal.addEventListener("modal:trigger", (e) => { document.getElementById("inq-space").textContent = e.detail.dataset.space; });
    document.getElementById("inquiry-form").addEventListener("submit", (e) => {
      e.preventDefault();
      if (!SN.validate(e.target)) return;
      SN.closeModal(modal);
      e.target.reset();
      SN.toast("Inquiry sent. The owner will contact you.");
    });
  });
})();
