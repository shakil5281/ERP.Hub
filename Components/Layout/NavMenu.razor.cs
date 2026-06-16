using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace ERPHub.Components.Layout;

public partial class NavMenu
{
    [Inject]
    private IJSRuntime JS { get; set; } = default!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    private async Task CloseSidebar()
    {
        await JS.InvokeVoidAsync("sidebarFunctions.close");
    }
}

