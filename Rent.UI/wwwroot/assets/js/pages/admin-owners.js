/* Admin: activate / deactivate owners (BR-24). Deactivation revokes the owner's refresh tokens on the server. */
(function () {
  "use strict";
  const SN = window.StayNest;
  document.addEventListener("change", async (e) => {
    const input = e.target.closest("input[data-owner]");
    if (!input) return;
    const name = input.dataset.owner;
    const activating = input.checked;
    input.checked = !activating; // revert until confirmed
    const ok = await SN.confirm({
      title: activating ? `Activate ${name}?` : `Deactivate ${name}?`,
      message: activating ? "They will be able to log in again." : "They will be signed out and cannot log in. Their data is kept.",
      confirmText: activating ? "Activate" : "Deactivate",
      danger: !activating,
    });
    if (!ok) return;
    input.checked = activating;
    const cell = input.closest("tr").querySelector("[data-status]");
    cell.innerHTML = activating ? '<span class="pill pill-green">Active</span>' : '<span class="pill pill-red">Inactive</span>';
    SN.toast(activating ? `${name} activated.` : `${name} deactivated and signed out.`);
  });
})();
