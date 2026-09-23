
"use strict";

document.addEventListener("DOMContentLoaded", () => {
    mostrarAvisos();
    document.querySelectorAll("select[data-razas-destino]").forEach(configurarCascada);
    document.querySelectorAll("[data-carga-foto]").forEach(configurarCargaFoto);
  
    document.querySelectorAll("select[data-envio-auto]").forEach((s) =>
        s.addEventListener("change", () => s.form.requestSubmit()));
});

function mostrarAvisos() {
    document.querySelectorAll(".toast[data-autoshow]").forEach((el) =>
        bootstrap.Toast.getOrCreateInstance(el).show());
}

function configurarCascada(especie) {
    const raza = document.querySelector(especie.dataset.razasDestino);
    if (!raza) return;

    const url = especie.dataset.razasUrl;
    const textoVacio = especie.dataset.razasVacio || "Elige una raza";
    let peticionActual = null;

    especie.addEventListener("change", async () => {
        const especieId = especie.value;
        peticionActual?.abort();

        raza.replaceChildren(new Option(especieId ? "Cargando razas…" : (raza.dataset.sinEspecie || textoVacio), ""));
        if (!especieId) {
            raza.dispatchEvent(new Event("change", { bubbles: true }));
            return;
        }

        raza.classList.add("select-cargando");
        peticionActual = new AbortController();

        try {
            const respuesta = await fetch(`${url}/${encodeURIComponent(especieId)}`, {
                headers: { Accept: "application/json" },
                signal: peticionActual.signal
            });
            if (!respuesta.ok) throw new Error(`HTTP ${respuesta.status}`);

            const razas = await respuesta.json();
            raza.replaceChildren(
                new Option(razas.length ? textoVacio : "Esta especie no tiene razas", ""),
                ...razas.map((r) => new Option(r.nombre, r.id))
            );
        } catch (error) {
            if (error.name === "AbortError") return;
            raza.replaceChildren(new Option("No se pudieron cargar las razas", ""));
            console.error("Error cargando razas:", error);
        } finally {
            raza.classList.remove("select-cargando");
            raza.dispatchEvent(new Event("change", { bubbles: true }));
        }
    });
}

function configurarCargaFoto(zona) {
    const input = zona.querySelector("input[type=file]");
    const vista = zona.querySelector("img");
    const quitar = document.querySelector(zona.dataset.quitar || "#quitarFoto");
    const original = vista?.getAttribute("src") || "";

    const pintar = (src) => {
        if (src) {
            vista.src = src;
            vista.hidden = false;
            zona.classList.add("con-foto");
        } else {
            vista.removeAttribute("src");
            vista.hidden = true;
            zona.classList.remove("con-foto");
        }
    };

    input.addEventListener("change", () => {
        const archivo = input.files?.[0];
        if (!archivo) { pintar(quitar?.checked ? "" : original); return; }
        if (!archivo.type.startsWith("image/")) { input.value = ""; return; }
        if (quitar) quitar.checked = false;
        pintar(URL.createObjectURL(archivo));
    });

    quitar?.addEventListener("change", () => {
        if (quitar.checked) { input.value = ""; pintar(""); }
        else pintar(original);
    });

    ["dragenter", "dragover"].forEach((ev) =>
        zona.addEventListener(ev, () => zona.classList.add("arrastrando")));
    ["dragleave", "drop"].forEach((ev) =>
        zona.addEventListener(ev, () => zona.classList.remove("arrastrando")));
}
