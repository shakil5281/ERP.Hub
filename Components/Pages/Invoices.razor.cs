using Microsoft.AspNetCore.Components;
using ERPHub.Models;
using ERPHub.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERPHub.Components.Pages;

public partial class Invoices
{
    [Inject] private IErpService ErpService { get; set; } = default!;

    private List<Invoice> _invoicesList = new();
    private Invoice _newInvoice = new();

    protected override async Task OnInitializedAsync()
    {
        ResetForm();
        await LoadInvoices();
    }

    private async Task LoadInvoices()
    {
        _invoicesList = await ErpService.GetInvoicesAsync();
    }

    private void ResetForm()
    {
        _newInvoice = new Invoice
        {
            InvoiceNumber = $"INV-2026-00{_invoicesList.Count + 1}",
            IssueDate = DateTime.Today,
            DueDate = DateTime.Today.AddDays(30),
            Status = InvoiceStatus.Draft,
            TaxRate = 0.15m
        };
        _newInvoice.Items.Add(new InvoiceItem { Description = "Consulting & Support Services", Quantity = 1, UnitPrice = 1500m });
    }

    private void AddLineItem()
    {
        _newInvoice.Items.Add(new InvoiceItem { Description = "", Quantity = 1, UnitPrice = 0m });
    }

    private void RemoveLineItem(int index)
    {
        if (_newInvoice.Items.Count > 0 && index < _newInvoice.Items.Count)
        {
            _newInvoice.Items.RemoveAt(index);
        }
    }

    private void OnTaxRateChanged(ChangeEventArgs e)
    {
        if (decimal.TryParse(e.Value?.ToString(), out var taxRate))
        {
            _newInvoice.TaxRate = taxRate;
        }
    }

    private async Task SaveInvoice()
    {
        await ErpService.AddInvoiceAsync(_newInvoice);
        await LoadInvoices();
        ResetForm();
    }

    private void CopyToDesigner(Invoice inv)
    {
        _newInvoice = new Invoice
        {
            Id = inv.Id,
            InvoiceNumber = inv.InvoiceNumber,
            ClientName = inv.ClientName,
            ClientEmail = inv.ClientEmail,
            IssueDate = inv.IssueDate,
            DueDate = inv.DueDate,
            Status = inv.Status,
            TaxRate = inv.TaxRate,
            DiscountAmount = inv.DiscountAmount,
            Items = inv.Items.Select(item => new InvoiceItem
            {
                Id = item.Id,
                Description = item.Description,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            }).ToList()
        };
    }

    private async Task DeleteInvoice(int id)
    {
        await ErpService.DeleteInvoiceAsync(id);
        await LoadInvoices();
    }
}
