/* Auth pages: login, register, change password */
(function () {
  "use strict";
  const SN = window.StayNest;

  function passwordChecks(v) {
    return { len: v.length >= 8, upper: /[A-Z]/.test(v), lower: /[a-z]/.test(v), digit: /\d/.test(v) };
  }
  function bindStrength(input) {
    const meter = document.querySelector(input.dataset.strength);
    const rules = document.querySelector(input.dataset.rules);
    input.addEventListener("input", () => {
      const c = passwordChecks(input.value);
      const level = Object.values(c).filter(Boolean).length;
      meter && (meter.dataset.level = String(level));
      rules && rules.querySelectorAll("li").forEach((li) => li.classList.toggle("ok", c[li.dataset.rule]));
    });
  }
  const strong = (v) => Object.values(passwordChecks(v)).every(Boolean);

  document.addEventListener("DOMContentLoaded", () => {
    document.querySelectorAll("[data-strength]").forEach(bindStrength);

    // Login: in the MVC app this posts to AccountController.Login, which calls the API
    // and stores the tokens in HttpOnly cookies. Here we simulate success.
    const login = document.getElementById("login-form");
    login && login.addEventListener("submit", (e) => {
      e.preventDefault();
      document.getElementById("login-error").classList.add("hidden");
      if (!SN.validate(login)) return;
      const email = login.querySelector("#l-email").value.trim().toLowerCase();
      if (login.querySelector("#l-pass").value === "wrong") {
        document.getElementById("login-error").classList.remove("hidden");
        return;
      }
      const target = email.startsWith("admin") ? "../admin/dashboard.html" : email.startsWith("ankit") ? "../tenant/dashboard.html" : "../owner/dashboard.html";
      window.location.href = target;
    });

    const reg = document.getElementById("register-form");
    reg && reg.addEventListener("submit", (e) => {
      e.preventDefault();
      let ok = SN.validate(reg);
      const pass = reg.querySelector("#r-pass");
      const strongOk = strong(pass.value);
      pass.closest(".field").classList.toggle("invalid", !strongOk);
      if (!ok || !strongOk) return;
      SN.toast("Account created. Log in to continue.");
      setTimeout(() => (window.location.href = "login.html"), 900);
    });

    const pw = document.getElementById("password-form");
    pw && pw.addEventListener("submit", (e) => {
      e.preventDefault();
      const ok = SN.validate(pw);
      const n = pw.querySelector("#c-new"), c = pw.querySelector("#c-confirm");
      const strongOk = strong(n.value);
      n.closest(".field").classList.toggle("invalid", !strongOk);
      const match = n.value === c.value && c.value !== "";
      c.closest(".field").classList.toggle("invalid", !match);
      if (!ok || !strongOk || !match) return;
      SN.toast("Password saved. Other devices were signed out.");
      setTimeout(() => (window.location.href = "../tenant/dashboard.html"), 900);
    });
  });
})();
