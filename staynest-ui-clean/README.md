# StayNest UI templates (draft)

Static HTML templates for StayNest v1. Open `index.html` in a browser to reach every page.
No build step and no frameworks. Icons come from one SVG sprite (`assets/icons/icons.svg`). Charts are drawn with SVG by `assets/js/common/charts.js`.

**Preview through a local web server**, not by double-clicking the files. Browsers block the icon sprite on `file://` pages, so icons would be missing. For example: `npx serve .` or `python -m http.server`, or the VS Code Live Server extension. Inside the MVC app (`wwwroot`) this works as is.

## Folder structure

```
staynest-ui/
  index.html                  Page index
  auth/                       login, register, change-password, logged-out
  owner/                      dashboard, properties, property-form, property-detail, space-status,
                              move-in, tenancies, tenancy-detail, invoices, inquiries,
                              reports-collections, reports-occupancy, account
  tenant/                     dashboard (my stay), invoices, documents, account
  admin/                      dashboard, owners, account
  public/                     search (vacancy search with inquiry form)
  errors/                     404, 403, 500
  assets/
    css/common/               variables.css (tokens), base.css, layout.css (app shell), components.css
    css/pages/                auth, dashboard, property-tree, space-status, move-in, search
    icons/icons.svg           SVG sprite used by every page (54 icons)
    icons/svg/                the same icons as single files, for reference and design tools
    js/common/                app.js (sidebar, menus, modals, confirm dialog, tabs, password toggle),
                              charts.js (SVG charts, loaded only on pages with charts)
    js/pages/                 move-in.js (wizard steps, co-occupant rows),
                              property-detail.js (space tree expand/collapse),
                              dropzone.js (drag highlight on the upload area)
```

## Moving into ASP.NET Core MVC

- Copy `assets/` into `wwwroot/`.
- The sidebar, top bar and script tags become `Views/Shared/_Layout.cshtml`, with the
  navigation in a partial per role (`_OwnerNav`, `_TenantNav`, `_AdminNav`).
- Auth pages use a separate `_AuthLayout.cshtml`.
- Each page's `<main class="content">` block becomes the view body. Page CSS and JS go in
  `@section Styles` and `@section Scripts`.
- Replace `<input name="__RequestVerificationToken" value="template-token">` with `@Html.AntiForgeryToken()`.
- Sample data (Sunrise Residency, Green Valley PG) matches the requirements document examples.

## Icons

Every icon is an `<svg>` that points into the sprite:

```html
<svg class="icon" viewBox="0 0 24 24" aria-hidden="true"><use href="../assets/icons/icons.svg#edit"/></svg>
```

- Size, stroke and colour come from the `.icon` CSS classes (`icon-sm`, `icon-lg`) and `currentColor`, as before.
- In Razor views, don't rely on `~/` being resolved inside `<use href>`. Use `@Url.Content("~/assets/icons/icons.svg")#edit`, or a root path like `/assets/icons/icons.svg#edit` if the app runs at the site root.
- Icon names: alert-triangle, apartment, bath, bed, bell, building, calendar, car, chart-pie, check, chevron-down, chevron-left, chevron-right, clock, close, door, download, edit, eye, eye-off, file, grid, hand, home, inbox, info, key, layers, lock, logo, logout, mail, map-pin, menu, more-vertical, phone, plus, receipt, refresh, rupee, search, send, shield, sliders, snowflake, sofa, trash, upload, user, user-plus, users, utensils, wifi, zap.
- To add an icon: add a `<symbol id="name" viewBox="0 0 24 24">…</symbol>` to `icons.svg` (and the single file to `icons/svg/`).

## JavaScript scope

The scripts only handle design and layout behaviour. There is no client-side validation, data loading,
filtering, sorting, calculation or simulated saving. Forms post normally; add validation yourself
(for example ASP.NET Core tag helpers with `asp-validation-for`).

- `.field-error` spans are kept as message slots. They show when the parent `.field` has the `invalid` class.
- `data-confirm` on a link navigates after confirmation; on a button inside a form it submits that form.
- Removed: ajax.js and the page scripts for account, admin-owners, auth, inquiries, invoices, reports,
  search, space-status and property-form; `required`, `novalidate`, `data-rule` and the data-only
  `data-*` attributes they used.
