window.sidebarFunctions = {
    toggle: function () {
        const sidebar = document.querySelector('.sidebar-panel');
        const backdrop = document.querySelector('.sidebar-backdrop');
        if (sidebar && backdrop) {
            sidebar.classList.toggle('open');
            backdrop.classList.toggle('show');
        }
    },
    close: function () {
        const sidebar = document.querySelector('.sidebar-panel');
        const backdrop = document.querySelector('.sidebar-backdrop');
        if (sidebar && backdrop) {
            sidebar.classList.remove('open');
            backdrop.classList.remove('show');
        }
    }
};

window.datepickerFunctions = {
    shouldOpenUp: function (element) {
        if (!element) return false;
        const rect = element.getBoundingClientRect();
        const viewportHeight = window.innerHeight;
        const spaceBelow = viewportHeight - rect.bottom;
        const calendarHeight = 320;
        return spaceBelow < calendarHeight && rect.top > calendarHeight;
    }
};

window.downloadFile = function (filename, contentType, base64Data) {
    const byteCharacters = atob(base64Data);
    const byteNumbers = new Array(byteCharacters.length);
    for (let i = 0; i < byteCharacters.length; i++) {
        byteNumbers[i] = byteCharacters.charCodeAt(i);
    }
    const byteArray = new Uint8Array(byteNumbers);
    const blob = new Blob([byteArray], { type: contentType });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = filename;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    URL.revokeObjectURL(url);
};

