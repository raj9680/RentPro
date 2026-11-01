/* ==========================================================================
   StayNest charts: small, dependency-free SVG charts driven by data attributes.
   Supported:
     data-chart="spark"  data-points="1,3,2" data-color="green|blue|red"
     data-chart="area"   data-points="..." data-highlight="4" data-tip="Rs 1,17,629"
     data-chart="donut"  data-segments='[{"value":12,"color":"green"}]' data-tip="12 occupied"
     data-chart="bars"   data-labels='["A","B"]' data-series='[{"name":"Billed","color":"blue","values":[1,2]}]' data-format="inr|pct|int"
   In the MVC app, set these attributes from the view model and keep this file.
   ========================================================================== */
(function () {
  "use strict";
  const NS = "http://www.w3.org/2000/svg";
  const css = (name) => getComputedStyle(document.documentElement).getPropertyValue(name).trim();
  const COLORS = {
    green: () => css("--green"), blue: () => css("--blue"), red: () => css("--red"),
    lightblue: () => css("--blue-light"), orange: () => css("--orange"), grey: () => "#D9D9DD",
  };
  const color = (c) => (COLORS[c] ? COLORS[c]() : c);
  let uid = 0;

  function el(tag, attrs = {}, parent) {
    const n = document.createElementNS(NS, tag);
    Object.entries(attrs).forEach(([k, v]) => n.setAttribute(k, v));
    parent && parent.appendChild(n);
    return n;
  }
  const nums = (s) => String(s).split(",").map(Number);

  // Catmull-Rom to cubic bezier for smooth lines
  function smoothPath(pts) {
    if (pts.length < 2) return "";
    let d = `M${pts[0][0]},${pts[0][1]}`;
    for (let i = 0; i < pts.length - 1; i++) {
      const p0 = pts[i - 1] || pts[i], p1 = pts[i], p2 = pts[i + 1], p3 = pts[i + 2] || p2;
      const c1x = p1[0] + (p2[0] - p0[0]) / 6, c1y = p1[1] + (p2[1] - p0[1]) / 6;
      const c2x = p2[0] - (p3[0] - p1[0]) / 6, c2y = p2[1] - (p3[1] - p1[1]) / 6;
      d += ` C${c1x},${c1y} ${c2x},${c2y} ${p2[0]},${p2[1]}`;
    }
    return d;
  }

  function scalePoints(values, w, h, padTop, padBottom) {
    const min = Math.min(...values), max = Math.max(...values);
    const span = max - min || 1;
    return values.map((v, i) => [
      (i / (values.length - 1)) * w,
      padTop + (1 - (v - min) / span) * (h - padTop - padBottom),
    ]);
  }

  function lineArea(node, opts) {
    node.innerHTML = "";
    const values = nums(node.dataset.points);
    const w = 100, h = 40;
    const c = color(node.dataset.color || "green");
    const pts = scalePoints(values, w, h, opts.padTop, opts.padBottom);
    const svg = el("svg", { viewBox: `0 0 ${w} ${h}`, preserveAspectRatio: "none", "aria-hidden": "true" }, node);
    const id = "g" + ++uid;
    const grad = el("linearGradient", { id, x1: 0, y1: 0, x2: 0, y2: 1 }, el("defs", {}, svg));
    el("stop", { offset: "0%", "stop-color": c, "stop-opacity": opts.fillOpacity }, grad);
    el("stop", { offset: "100%", "stop-color": c, "stop-opacity": 0 }, grad);
    const line = smoothPath(pts);
    el("path", { d: `${line} L${w},${h} L0,${h} Z`, fill: `url(#${id})` }, svg);
    el("path", { d: line, fill: "none", stroke: c, "stroke-width": opts.stroke, "vector-effect": "non-scaling-stroke", "stroke-linecap": "round" }, svg);
    return { pts, w, h };
  }

  function spark(node) {
    lineArea(node, { padTop: 4, padBottom: 6, fillOpacity: 0.35, stroke: 1.6 });
  }

  function area(node) {
    const { pts, w, h } = lineArea(node, { padTop: 10, padBottom: 4, fillOpacity: 0.28, stroke: 1.8 });
    const hi = node.dataset.highlight;
    if (hi === undefined) return;
    const p = pts[Number(hi)];
    const left = (p[0] / w) * 100 + "%";
    const top = (p[1] / h) * 100 + "%";
    const guide = document.createElement("div");
    guide.style.cssText = `position:absolute;left:${left};top:${top};bottom:0;border-left:1px dashed #C9C9CE;`;
    const dot = document.createElement("div");
    dot.style.cssText = `position:absolute;left:${left};top:${top};width:10px;height:10px;margin:-5px 0 0 -5px;border-radius:50%;background:${color("blue")};box-shadow:0 0 0 3px #fff;`;
    node.appendChild(guide);
    node.appendChild(dot);
    if (node.dataset.tip) {
      const tip = document.createElement("div");
      tip.className = "chart-tip";
      tip.style.left = left;
      tip.style.top = top;
      tip.textContent = node.dataset.tip;
      node.appendChild(tip);
    }
  }

  function donut(node) {
    node.innerHTML = "";
    const segs = JSON.parse(node.dataset.segments);
    const total = segs.reduce((a, s) => a + s.value, 0) || 1;
    const size = 200, r = 70, sw = 16, cx = 100, cy = 100;
    const circ = 2 * Math.PI * r;
    const gap = segs.length > 1 ? 10 : 0; // px along the circumference
    const svg = el("svg", { viewBox: `0 0 ${size} ${size}`, role: "img", "aria-label": node.dataset.label || "Donut chart" }, node);
    el("circle", { cx, cy, r, fill: "none", stroke: css("--track"), "stroke-width": sw }, svg);
    let offset = 0;
    let firstEnd = null;
    segs.forEach((s, i) => {
      const len = Math.max((s.value / total) * circ - gap, 0.01);
      el("circle", {
        cx, cy, r, fill: "none", stroke: color(s.color), "stroke-width": sw, "stroke-linecap": "round",
        "stroke-dasharray": `${len} ${circ - len}`, "stroke-dashoffset": -offset,
        transform: `rotate(-90 ${cx} ${cy})`,
      }, svg);
      if (i === Number(node.dataset.tipIndex || 0)) firstEnd = offset + len * 0.8;
      offset += len + gap;
    });
    if (node.dataset.tip && firstEnd !== null) {
      const ang = (firstEnd / circ) * 2 * Math.PI - Math.PI / 2;
      const x = cx + r * Math.cos(ang), y = cy + r * Math.sin(ang);
      const tip = document.createElement("div");
      tip.className = "chart-tip";
      tip.style.left = (x / size) * 100 + "%";
      tip.style.top = (y / size) * 100 + "%";
      tip.style.transform = "translate(-10%, -120%)";
      tip.textContent = node.dataset.tip;
      node.appendChild(tip);
    }
    if (node.dataset.center) {
      const c = document.createElement("div");
      c.style.cssText = "position:absolute;inset:0;display:grid;place-items:center;text-align:center;pointer-events:none";
      c.innerHTML = node.dataset.center;
      node.appendChild(c);
    }
  }

  function fmt(v, f) {
    if (f === "inr") return "\u20B9" + Number(v).toLocaleString("en-IN");
    if (f === "pct") return v + "%";
    return String(v);
  }
  function short(v, f) {
    if (f === "inr") return v >= 100000 ? "\u20B9" + (v / 100000).toFixed(1) + "L" : v >= 1000 ? "\u20B9" + Math.round(v / 1000) + "k" : "\u20B9" + v;
    if (f === "pct") return v + "%";
    return String(v);
  }

  function bars(node) {
    node.innerHTML = "";
    const labels = JSON.parse(node.dataset.labels);
    const series = JSON.parse(node.dataset.series);
    const f = node.dataset.format || "int";
    const W = node.clientWidth || 600, H = node.clientHeight || 220;
    const padL = 48, padB = 28, padT = 10;
    const max = node.dataset.max ? Number(node.dataset.max) : Math.max(...series.flatMap((s) => s.values)) * 1.15;
    const svg = el("svg", { viewBox: `0 0 ${W} ${H}`, role: "img", "aria-label": node.dataset.label || "Bar chart" }, node);
    const ticks = 4;
    for (let i = 0; i <= ticks; i++) {
      const y = padT + (i / ticks) * (H - padT - padB);
      el("line", { x1: padL, x2: W, y1: y, y2: y, stroke: css("--line"), "stroke-width": 1 }, svg);
      const t = el("text", { x: padL - 8, y: y + 4, "text-anchor": "end", "font-size": 11, fill: css("--muted") }, svg);
      t.textContent = short(Math.round(max * (1 - i / ticks)), f);
    }
    const groupW = (W - padL) / labels.length;
    const barW = Math.min(22, (groupW * 0.6) / series.length);
    labels.forEach((lab, gi) => {
      const gx = padL + gi * groupW + groupW / 2;
      series.forEach((s, si) => {
        const v = s.values[gi];
        const bh = (v / max) * (H - padT - padB);
        const x = gx - (series.length * barW) / 2 + si * barW + 2;
        const r = el("rect", { x, y: H - padB - bh, width: barW - 4, height: Math.max(bh, 1), rx: 4, fill: color(s.color) }, svg);
        const title = el("title", {}, r);
        title.textContent = `${s.name}, ${lab}: ${fmt(v, f)}`;
      });
      const t = el("text", { x: gx, y: H - 8, "text-anchor": "middle", "font-size": 11.5, fill: css("--text"), "font-weight": 500 }, svg);
      t.textContent = lab;
    });
  }

  const RENDER = { spark, area, donut, bars };
  function renderAll() {
    document.querySelectorAll("[data-chart]").forEach((n) => {
      const fn = RENDER[n.dataset.chart];
      fn && fn(n);
    });
  }
  window.StayNest = window.StayNest || {};
  window.StayNest.renderCharts = renderAll;
  document.addEventListener("DOMContentLoaded", renderAll);
  let t;
  window.addEventListener("resize", () => {
    clearTimeout(t);
    t = setTimeout(() => document.querySelectorAll('[data-chart="bars"]').forEach(bars), 150);
  });
})();
