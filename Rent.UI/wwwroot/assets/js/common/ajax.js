/* ==========================================================================
   StayNest Ajax helper (for the MVC app)
   - Sends the ASP.NET Core anti-forgery token as the RequestVerificationToken header
   - Handles common status codes from the Web (BFF) controllers
   Browser JavaScript calls MVC controller actions only, never the Web API directly.
   In these static templates the calls are not used; page scripts simulate responses.
   ========================================================================== */
(function () {
  "use strict";
  const SN = (window.StayNest = window.StayNest || {});

  function antiForgeryToken() {
    const input = document.querySelector('input[name="__RequestVerificationToken"]');
    return input ? input.value : "";
  }

  async function request(method, url, body, isForm) {
    const headers = { "X-Requested-With": "XMLHttpRequest" };
    if (method !== "GET") headers["RequestVerificationToken"] = antiForgeryToken();
    if (body && !isForm) headers["Content-Type"] = "application/json";

    const res = await fetch(url, {
      method,
      headers,
      credentials: "same-origin",
      body: body ? (isForm ? body : JSON.stringify(body)) : undefined,
    });

    if (res.status === 401) { window.location.href = "/Account/Login?returnUrl=" + encodeURIComponent(location.pathname); return null; }
    if (res.status === 204) return { ok: true };

    const type = res.headers.get("Content-Type") || "";
    const data = type.includes("json") ? await res.json() : await res.text();

    if (res.ok) return { ok: true, data };

    // ProblemDetails from the API is passed through by the Web controllers
    const problem = typeof data === "object" ? data : { title: "Something went wrong" };
    if (res.status === 400 && problem.errors) return { ok: false, status: 400, errors: problem.errors, problem };
    if (res.status === 409) SN.toast(problem.detail || "This record was changed by someone else. Refresh and try again.", "error");
    else if (res.status === 429) SN.toast("Too many requests. Wait a moment and try again.", "error");
    else if (res.status === 403) SN.toast("You do not have access to this action.", "error");
    else if (res.status >= 500) SN.toast("The server could not complete the request. Reference: " + (problem.traceId || "n/a"), "error");
    return { ok: false, status: res.status, problem };
  }

  /* Show 400 validation errors next to fields: { "Email": ["Email is already registered."] } */
  SN.showFieldErrors = function (form, errors) {
    Object.entries(errors || {}).forEach(([name, messages]) => {
      const input = form.querySelector(`[name="${name}"]`);
      const field = input && input.closest(".field");
      if (!field) return;
      field.classList.add("invalid");
      const err = field.querySelector(".field-error");
      if (err) err.textContent = messages[0];
    });
  };

  SN.ajax = {
    get: (url) => request("GET", url),
    post: (url, body) => request("POST", url, body),
    put: (url, body) => request("PUT", url, body),
    del: (url) => request("DELETE", url),
    upload: (url, formData) => request("POST", url, formData, true),
    /* Load an MVC partial view into a container */
    loadPartial: async (url, container) => {
      const r = await request("GET", url);
      if (r && r.ok) container.innerHTML = r.data;
      return r;
    },
  };
})();
