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



