window.erpToast = {
    show: function (type, title, message) {
        var container = document.querySelector('.toast-container');
        if (!container) {
            container = document.createElement('div');
            container.className = 'toast-container';
            document.body.appendChild(container);
        }

        var icons = {
            success: 'check_circle',
            error: 'error',
            warning: 'warning',
            info: 'info'
        };

        var toast = document.createElement('div');
        toast.className = 'toast ' + type;
        toast.innerHTML =
            '<span class="material-icons toast-icon">' + (icons[type] || 'info') + '</span>' +
            '<div class="toast-content">' +
            '  <div class="toast-title">' + this._escape(title) + '</div>' +
            '  <div class="toast-message">' + this._escape(message) + '</div>' +
            '</div>' +
            '<button class="toast-close" onclick="this.parentElement.classList.add(\'out\');setTimeout(function(){this.parentElement.remove()}.bind(this),200)">&times;</button>';

        container.appendChild(toast);

        setTimeout(function () {
            toast.classList.add('out');
            setTimeout(function () { toast.remove(); }, 200);
        }, 4000);
    },

    _escape: function (text) {
        var div = document.createElement('div');
        div.appendChild(document.createTextNode(text));
        return div.innerHTML;
    }
};
