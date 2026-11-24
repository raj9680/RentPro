/* Property detail (UI only): collapsible space tree with expand all / collapse all.
   Rows use data-id and data-parent to know their place in the tree. */
(function () {
  "use strict";

  function childrenOf(id) {
    return Array.from(document.querySelectorAll('.tree-row[data-parent="' + id + '"]'));
  }

  function setVisible(row, visible) {
    childrenOf(row.dataset.id).forEach((child) => {
      child.classList.toggle("hidden-child", !visible);
      const childOpen = !child.classList.contains("collapsed");
      setVisible(child, visible && childOpen);
    });
  }

  function setAll(open) {
    document.querySelectorAll(".tree-row[data-id]").forEach((r) => {
      r.classList.toggle("collapsed", !open);
      r.classList.toggle("hidden-child", !open && r.dataset.parent !== "");
      const toggle = r.querySelector(".tree-toggle");
      if (toggle) toggle.setAttribute("aria-expanded", String(open));
    });
  }

  document.addEventListener("DOMContentLoaded", () => {
    document.querySelectorAll(".tree-toggle").forEach((btn) => {
      btn.addEventListener("click", () => {
        const row = btn.closest(".tree-row");
        const collapsed = row.classList.toggle("collapsed");
        btn.setAttribute("aria-expanded", String(!collapsed));
        setVisible(row, !collapsed);
      });
    });

    const expandAll = document.getElementById("expand-all");
    const collapseAll = document.getElementById("collapse-all");
    if (expandAll) expandAll.addEventListener("click", () => setAll(true));
    if (collapseAll) collapseAll.addEventListener("click", () => setAll(false));
  });
})();
