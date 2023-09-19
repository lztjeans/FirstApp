﻿using Microsoft.AspNetCore.Mvc.Rendering;
using FirstApp.ViewModels;
using FirstApp.Services;
using FirstApp.Models;
using FirstApp.Specifications;
using NLog;
using Microsoft.AspNetCore.Mvc;
using FirstApp.Constants;
using Microsoft.AspNetCore.Identity;

namespace FirstApp.Service;


public class CatalogViewModelService : ICatalogViewModelService
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private readonly IRepository<CatalogItem> _itemRepository;
    private readonly IRepository<CatalogBrand> _brandRepository;
    private readonly IRepository<CatalogType> _typeRepository;
    private readonly IUriComposer _uriComposer;

    public CatalogViewModelService(
        IRepository<CatalogItem> itemRepository,
        IRepository<CatalogBrand> brandRepository,
        IRepository<CatalogType> typeRepository,
        IUriComposer uriComposer)
    {
        _itemRepository = itemRepository;
        _brandRepository = brandRepository;
        _typeRepository = typeRepository;
        _uriComposer = uriComposer;
    }

    public async Task<CatalogIndexViewModel> GetCatalogItems(int pageIndex, int itemsPage, int? brandId, int? typeId)
    {
        //
        //Task<>
        _logger.Info("GetCatalogItems called.");

        var filterSpecification = new CatalogFilterSpecification(brandId, typeId);
        var filterPaginatedSpecification =
            new CatalogFilterPaginatedSpecification(itemsPage * pageIndex, itemsPage, brandId, typeId);

        // the implementation below using ForEach and Count. We need a List.
        //_itemRepository.
        var itemsOnPage = await _itemRepository.ListAsync(filterPaginatedSpecification);
        var totalItems = await _itemRepository.CountAsync(filterSpecification);

        CatalogIndexViewModel vm = new()
        {
            //itemsOnPage.
            CatalogItems = itemsOnPage.Select(i => new CatalogItemViewModel()
            {
                Id = i.Id,
                Name = i.Name,
                PictureUri = _uriComposer.ComposePicUri(i.PictureUri),
                Price = i.Price
            }).ToList(),
            Brands = (await GetBrands()).ToList(),
            Types = (await GetTypes()).ToList(),
            BrandFilterApplied = brandId ?? 0,
            TypesFilterApplied = typeId ?? 0,
            PaginationInfo = new PaginationInfoViewModel()
            {
                ActualPage = pageIndex,
                ItemsPerPage = itemsOnPage.Count,
                TotalItems = totalItems,
                TotalPages = int.Parse(Math.Ceiling(((decimal)totalItems / itemsPage)).ToString())
            }
        };

        vm.PaginationInfo.Next = (vm.PaginationInfo.ActualPage == vm.PaginationInfo.TotalPages - 1) ? "is-disabled" : "";
        vm.PaginationInfo.Previous = (vm.PaginationInfo.ActualPage == 0) ? "is-disabled" : "";

        return vm;
    }

    public async Task<IEnumerable<SelectListItem>> GetBrands()
    {
        _logger.Info("GetBrands called.");
        var brands = await _brandRepository.ListAsync();

        var items = brands
            .Select(brand => new SelectListItem() { Value = brand.Id.ToString(), Text = brand.Brand })
            .OrderBy(b => b.Text)
            .ToList();

        var allItem = new SelectListItem() { Value = null, Text = "All", Selected = true };
        items.Insert(0, allItem);

        return items;
    }

    public async Task<IEnumerable<SelectListItem>> GetTypes()
    {
        _logger.Info("GetTypes called.");
        var types = await _typeRepository.ListAsync();

        var items = types
            .Select(type => new SelectListItem() { Value = type.Id.ToString(), Text = type.Type })
            .OrderBy(t => t.Text)
            .ToList();

        var allItem = new SelectListItem() { Value = null, Text = "All", Selected = true };
        items.Insert(0, allItem);

        return items;
    }

    public async Task<IEnumerable<CatalogItemViewModel>> GetAllItems()
    {
        List< CatalogItem> ents = await _itemRepository.ListAsync();

        var ret = new List< CatalogItemViewModel>();
        ents.ForEach( ent => {
            var e = new CatalogItemViewModel(ent)
            {
                BrandName = _brandRepository.GetByIdAsync(ent.CatalogBrandId).Result.Brand ,
                TypeName= _typeRepository.GetByIdAsync(ent.CatalogTypeId).Result.Type, //,
                BrandId = ent.CatalogBrandId,
                TypeId = ent.CatalogTypeId
            };
            ret.Add(e);
        });
        //throw new NotImplementedException();
        return ret;
    }

    public async Task<CatalogItemViewModel> GetItemById(int id)
    {

        CatalogItem ent = await _itemRepository.GetByIdAsync(id) ?? throw new DataNotFoundException(""+id);
        return new CatalogItemViewModel(ent);
    }

    public CatalogItemViewModel CreateItem(CatalogItem entity)
    {
        try
        {
             _itemRepository.AddAsync(entity);
            _itemRepository.SaveChangesAsync();
            return  new CatalogItemViewModel(entity);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public void DeleteItem(CatalogItem entity)
    {
        try
        {
             _itemRepository.DeleteAsync(entity);
            _itemRepository.SaveChangesAsync();
        }
        catch (Exception)
        {
            throw;
        }
    }
}

/*
 private async Task<BasketViewModel> CreateBasketForUser(string userId)
{
    var basket = new Basket(userId);
    _basketRepository.AddAsync(basket);

    return new BasketViewModel()
    {
        BuyerId = basket.BuyerId,
        Id = basket.Id,
    };
} 
*/