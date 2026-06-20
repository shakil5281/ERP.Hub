const modal = document.getElementById("components-reconnect-modal");
if (modal) {
    modal.addEventListener("components-reconnect-state-changed", onStateChanged);
}

document.getElementById("components-reconnect-button")?.addEventListener("click", retryReconnect);
document.getElementById("components-resume-button")?.addEventListener("click", resumeCircuit);
document.getElementById("components-retry-resume-button")?.addEventListener("click", retryResume);

function onStateChanged(event) {
    const state = event.detail?.state;
    if (state === "rejected") {
        location.reload();
    }
}

async function retryReconnect() {
    try {
        const ok = await Blazor.reconnect();
        if (!ok) {
            const resumed = await Blazor.resumeCircuit();
            if (!resumed) location.reload();
        }
    } catch {
        /* will retry automatically */
    }
}

async function resumeCircuit() {
    try {
        const ok = await Blazor.resumeCircuit();
        if (!ok) location.reload();
    } catch {
        if (modal) {
            modal.classList.remove("components-reconnect-paused");
            modal.classList.add("components-reconnect-resume-failed");
        }
    }
}

async function retryResume() {
    try {
        const ok = await Blazor.resumeCircuit();
        if (!ok) location.reload();
    } catch {
        location.reload();
    }
}
