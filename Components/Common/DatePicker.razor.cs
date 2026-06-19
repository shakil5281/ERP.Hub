using Microsoft.AspNetCore.Components;
using System.Globalization;

namespace ERPHub.Components.Common;

public partial class DatePicker
{
    [Parameter] public DateTime Value { get; set; } = DateTime.Today;
    [Parameter] public EventCallback<DateTime> ValueChanged { get; set; }

    private DateTime? _dateValue
    {
        get => Value;
        set
        {
            var newVal = value ?? DateTime.Today;
            if (newVal != Value)
            {
                Value = newVal;
                ValueChanged.InvokeAsync(newVal);
            }
        }
    }

    private static readonly CultureInfo _culture = new("en-US");
}
