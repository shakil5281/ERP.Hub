using Microsoft.JSInterop;

namespace ERPHub.Services
{
    public class ToastService
    {
        private readonly IJSRuntime _js;

        public ToastService(IJSRuntime js)
        {
            _js = js;
        }

        public async Task ShowSuccessAsync(string message, string title = "Success")
        {
            await _js.InvokeVoidAsync("erpToast.show", "success", title, message);
        }

        public async Task ShowErrorAsync(string message, string title = "Error")
        {
            await _js.InvokeVoidAsync("erpToast.show", "error", title, message);
        }

        public async Task ShowInfoAsync(string message, string title = "Info")
        {
            await _js.InvokeVoidAsync("erpToast.show", "info", title, message);
        }

        public async Task ShowWarningAsync(string message, string title = "Warning")
        {
            await _js.InvokeVoidAsync("erpToast.show", "warning", title, message);
        }
    }
}
