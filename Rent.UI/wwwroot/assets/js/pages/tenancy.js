/* Tenancy pages: notice date (BR-12), move-out confirm, document upload with validation (BR-20). */
(function () {
  "use strict";
  const SN = window.StayNest;
  const MONTHS = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
  const fmt = (d) => `${String(d.getDate()).padStart(2, "0")} ${MONTHS[d.getMonth()]} ${d.getFullYear()}`;
  const iso = (d) => `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, "0")}-${String(d.getDate()).padStart(2, "0")}`;
  const NOTICE_DAYS = 30;
  const MAX_BYTES = 5 * 1024 * 1024;
  const TYPES = ["application/pdf", "image/jpeg", "image/png"];

  document.addEventListener("DOMContentLoaded", () => {
    // Notice
    const nd = document.getElementById("notice-date");
    const ne = document.getElementById("notice-end");
    if (nd) {
      const update = () => {
        if (!nd.value) { ne.textContent = "-"; return; }
        const d = new Date(nd.value + "T00:00:00");
        d.setDate(d.getDate() + NOTICE_DAYS);
        ne.textContent = fmt(d);
      };
      nd.value = iso(new Date());
      nd.addEventListener("input", update);
      update();
      document.getElementById("notice-form").addEventListener("submit", (e) => {
        e.preventDefault();
        if (!SN.validate(e.target)) return;
        SN.closeModal("notice-modal");
        SN.toast(`Notice recorded. Planned move-out ${ne.textContent}.`);
      });
    }

    // Move-out confirm
    document.addEventListener("confirmed", (e) => {
      if (e.target.id === "move-out-btn") SN.toast("Move-out confirmed. The space is now vacant.");
    });

    // Upload (simulated progress; MVC uses StayNest.ajax.upload with FormData)
    const input = document.getElementById("doc-input");
    if (!input) return;
    const zone = document.getElementById("dropzone");
    const wrap = document.getElementById("upload-progress");
    const bar = wrap.querySelector(".progress-bar");
    const label = document.getElementById("upload-label");
    const list = document.getElementById("doc-list");

    ["dragenter", "dragover"].forEach((ev) => zone.addEventListener(ev, () => zone.classList.add("drag")));
    ["dragleave", "drop"].forEach((ev) => zone.addEventListener(ev, () => zone.classList.remove("drag")));

    input.addEventListener("change", () => {
      const file = input.files[0];
      if (!file) return;
      if (!TYPES.includes(file.type)) { SN.toast("Only PDF, JPG or PNG files can be uploaded.", "error"); input.value = ""; return; }
      if (file.size > MAX_BYTES) { SN.toast("File is larger than 5 MB.", "error"); input.value = ""; return; }
      wrap.classList.remove("hidden");
      let p = 0;
      const timer = setInterval(() => {
        p = Math.min(100, p + 12);
        bar.style.width = p + "%";
        label.textContent = `Uploading ${file.name} (${p}%)`;
        if (p === 100) {
          clearInterval(timer);
          const size = file.size > 1048576 ? (file.size / 1048576).toFixed(1) + " MB" : Math.max(1, Math.round(file.size / 1024)) + " KB";
          const row = document.createElement("div");
          row.className = "file-row";
          row.innerHTML = `<span class="file-icon"><svg class="icon" viewBox="0 0 24 24" aria-hidden="true"><path d="M14 3H6a1 1 0 0 0-1 1v16a1 1 0 0 0 1 1h12a1 1 0 0 0 1-1V8z"/><path d="M14 3v5h5"/></svg></span><div class="file-meta"><strong></strong><span></span></div>`;
          row.querySelector("strong").textContent = file.name;
          row.querySelector(".file-meta span").textContent = `${document.getElementById("doc-type").value}, ${size}, uploaded just now`;
          list.prepend(row);
          setTimeout(() => wrap.classList.add("hidden"), 400);
          input.value = "";
          SN.toast("Document uploaded.");
        }
      }, 120);
    });
  });
})();
