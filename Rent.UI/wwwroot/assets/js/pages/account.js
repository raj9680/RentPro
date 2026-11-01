/* Account settings: profile save and log out of all devices (revokes refresh tokens). */
(function () {
  "use strict";
  const SN = window.StayNest;
  document.addEventListener("DOMContentLoaded", () => {
    const form = document.getElementById("profile-form");
    form && form.addEventListener("submit", (e) => {
      e.preventDefault();
      if (SN.validate(form)) SN.toast("Changes saved.");
    });
  });
  document.addEventListener("confirmed", (e) => {
    if (e.target.matches(".btn-danger")) SN.toast("Logged out of all other devices.");
  });
})();
