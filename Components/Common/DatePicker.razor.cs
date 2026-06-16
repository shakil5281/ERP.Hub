using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace ERPHub.Components.Common;

public partial class DatePicker
{
    [Parameter] public DateTime Value { get; set; } = DateTime.Today;
    [Parameter] public EventCallback<DateTime> ValueChanged { get; set; }

    private bool _showCalendar = false;
    private DateTime _activeMonth = DateTime.Today;
    
    private int _daysInMonth;
    private int _dayOffset;

    protected override void OnInitialized()
    {
        _activeMonth = new DateTime(Value.Year, Value.Month, 1);
        CalculateMonthDetails();
    }

    protected override void OnParametersSet()
    {
        // sync calendar window with external value updates
        _activeMonth = new DateTime(Value.Year, Value.Month, 1);
        CalculateMonthDetails();
    }

    private void CalculateMonthDetails()
    {
        _daysInMonth = DateTime.DaysInMonth(_activeMonth.Year, _activeMonth.Month);
        _dayOffset = (int)new DateTime(_activeMonth.Year, _activeMonth.Month, 1).DayOfWeek;
    }

    [Inject] private IJSRuntime JS { get; set; } = default!;

    private ElementReference _wrapperElement;
    private bool _openUp = false;

    private async Task ToggleCalendar()
    {
        if (!_showCalendar)
        {
            try
            {
                _openUp = await JS.InvokeAsync<bool>("datepickerFunctions.shouldOpenUp", _wrapperElement);
            }
            catch
            {
                _openUp = false;
            }
        }
        _showCalendar = !_showCalendar;
    }

    private void CloseCalendar()
    {
        _showCalendar = false;
    }

    private void PrevMonth()
    {
        _activeMonth = _activeMonth.AddMonths(-1);
        CalculateMonthDetails();
    }

    private void NextMonth()
    {
        _activeMonth = _activeMonth.AddMonths(1);
        CalculateMonthDetails();
    }

    private async Task SelectDate(int day)
    {
        var selected = new DateTime(_activeMonth.Year, _activeMonth.Month, day);
        Value = selected;
        await ValueChanged.InvokeAsync(selected);
        _showCalendar = false;
    }
}
