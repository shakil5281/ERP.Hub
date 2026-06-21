using Microsoft.AspNetCore.Components;
using ERPHub.Models;
using ERPHub.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERPHub.Components.Pages;

public partial class Products
{
    [Inject] private IErpService ErpService { get; set; } = default!;

    private List<Product> _products = new();
    private List<string> _categories = new() { "Hardware", "Office Equipment", "Accessories", "Software License", "Services" };

    private string _searchQuery = string.Empty;
    private string _categoryFilter = string.Empty;

    private bool _showFormModal = false;
    private bool _isEditMode = false;
    private Product _editingProduct = new();

    protected override async Task OnInitializedAsync()
    {
        await LoadProducts();
    }

    private async Task LoadProducts()
    {
        _products = await ErpService.GetProductsAsync();
    }

    private IEnumerable<Product> FilteredProducts => _products
        .Where(p => (string.IsNullOrWhiteSpace(_searchQuery) ||
                     p.Name.Contains(_searchQuery, StringComparison.OrdinalIgnoreCase) ||
                     p.Sku.Contains(_searchQuery, StringComparison.OrdinalIgnoreCase)) &&
                    (string.IsNullOrWhiteSpace(_categoryFilter) || p.Category == _categoryFilter));

    private void OpenAddModal()
    {
        _isEditMode = false;
        _editingProduct = new Product { Category = "Hardware", LastUpdated = DateTime.Today };
        _showFormModal = true;
    }

    private void OpenEditModal(Product product)
    {
        _isEditMode = true;
        _editingProduct = new Product
        {
            Id = product.Id,
            Name = product.Name,
            Sku = product.Sku,
            Category = product.Category,
            Price = product.Price,
            Stock = product.Stock,
            Description = product.Description,
            LastUpdated = product.LastUpdated
        };
        _showFormModal = true;
    }

    private void CloseModal()
    {
        _showFormModal = false;
    }

    private async Task SaveProduct()
    {
        if (_isEditMode)
        {
            await ErpService.UpdateProductAsync(_editingProduct);
        }
        else
        {
            await ErpService.AddProductAsync(_editingProduct);
        }

        await LoadProducts();
        _showFormModal = false;
    }

    private async Task ConfirmDelete(int id)
    {
        await ErpService.DeleteProductAsync(id);
        await LoadProducts();
    }
}
