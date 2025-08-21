
    async function downloadCv(url, fileName) {
    try {
    const resolvedUrl = new URL(url, document.baseURI).href;

    const response = await fetch(resolvedUrl, { cache: 'no-store' });
    if (!response.ok) throw new Error(`Fehler beim Abrufen der Datei: ${response.status}`);

    const blob = await response.blob();
    const objectUrl = window.URL.createObjectURL(blob);

    const a = document.createElement("a");
    a.href = objectUrl;
    a.download = fileName || "CV.pdf";
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    window.URL.revokeObjectURL(objectUrl);
} catch (error) {
    console.error("CV-Download fehlgeschlagen:", error);
    // Fallback: im Tab öffnen (z. B. für iOS Safari)
    try { window.open(url, "_blank"); } catch {}
}
}
    window.downloadCv = downloadCv;


// Initialize
window.initTerminal = function () {
    window.terminalReady = true;

    const input = document.querySelector('.terminal-input input');
    if (input) {
        input.addEventListener('keydown', (e) => {
            if (e.key === 'Tab') {
                e.preventDefault();
            }
        });

        input.addEventListener('keyup', (e) => {
            if (e.key === 'Tab') {
                setTimeout(() => input.focus(), 0);
            }
        });
    }

    console.log('Terminal initialized');
};



// Text animation
window.animateText = function(text, elementId, speed = 50) {
    return new Promise(resolve => {
        const element = document.getElementById(elementId);
        if (!element) {
            resolve();
            return;
        }
        
        element.textContent = '';
        let i = 0;
        
        const interval = setInterval(() => {
            if (i < text.length) {
                element.textContent += text.charAt(i);
                i++;
            } else {
                clearInterval(interval);
                resolve();
            }
        }, speed);
    });
};
window.scrollTerminalToBottom = function () {
    const el = document.getElementById('terminal-body');
    if (el) {
        el.scrollTop = el.scrollHeight;
    }
};
