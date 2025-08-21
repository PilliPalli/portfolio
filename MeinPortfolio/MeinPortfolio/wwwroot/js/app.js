async function downloadCv() {
    try {
        const response = await fetch('cv/cv.pdf', { cache: 'no-store' });
        if (!response.ok) throw new Error(`Fehler beim Abrufen der Datei: ${response.status}`);

        const blob = await response.blob();
        const url = window.URL.createObjectURL(blob);

        const a = document.createElement("a");
        a.href = url;
        a.download = "My_CV.pdf"; 
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        window.URL.revokeObjectURL(url);
    } catch (error) {
        console.error("CV-Download fehlgeschlagen:", error);
    }
}

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
