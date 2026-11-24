/* Upload drop zone (UI only): highlight the zone while a file is dragged over it.
   No file checks and no upload; the <input type="file"> posts with its form. */
(function () {
  "use strict";

  document.addEventListener("DOMContentLoaded", () => {
    const zone = document.getElementById("dropzone");
    if (!zone) return;
    ["dragenter", "dragover"].forEach((ev) => zone.addEventListener(ev, () => zone.classList.add("drag")));
    ["dragleave", "drop"].forEach((ev) => zone.addEventListener(ev, () => zone.classList.remove("drag")));
  });
})();
