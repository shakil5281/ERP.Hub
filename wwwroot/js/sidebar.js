window.sidebarFunctions = {
    toggle: function () {
        const sidebar = document.querySelector('.sidebar');
        const backdrop = document.querySelector('.sidebar-backdrop');
        if (sidebar) {
            sidebar.classList.toggle('open');
            if (backdrop) backdrop.classList.toggle('show');
        }
    },
    close: function () {
        const sidebar = document.querySelector('.sidebar');
        const backdrop = document.querySelector('.sidebar-backdrop');
        if (sidebar) {
            sidebar.classList.remove('open');
            if (backdrop) backdrop.classList.remove('show');
        }
    }
};
