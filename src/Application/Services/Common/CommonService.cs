using Application.Helpers;
using Application.Interfaces;
using Application.Interfaces.Common;
using Domain.Dtos;
using Domain.Entities;
using Domain.Models;

namespace Application.Services.Common;

public class CommonService : ICommonService
{
    private readonly ISapService _sapService;
    private readonly IFileLogger _logger;

    public CommonService(ISapService sapService, IFileLogger logger)
    {
        _sapService = sapService;
        _logger = logger;
    }

    public async Task<Recipe> GetRecipeToAddAsync(BillOfMaterialHeaderDto dto)
    {
        try
        {
            // Si no existe una receta, retorna null
            var first = dto?.Results?.FirstOrDefault(r => !string.IsNullOrWhiteSpace(r.Material));
            if (first == null)
            {
                await _logger.LogErrorAsync($"No existe receta en la consulta a SAP", "Metodo: GetRecipeToAddAsync");
                return null;
            }

            var billOfMaterialHeaderUUID = Guid.TryParse(first.BillOfMaterialHeaderUUID, out var guid) ? guid : Guid.Empty;

            //  Creamos la nueva receta
            var recipe = new Recipe
            {
                BillOfMaterialHeaderUuid = billOfMaterialHeaderUUID,
                Material = first.Material,
                BillOfMaterial = first.BillOfMaterial,
                InterfaceCreateTimestamp = DateTime.Now,
                CommStatus = 1
            };


            // Consulta a SAP por los componentes o items de la receta
            var billOfMaterialItemUrl = $"https://sapfioriqas.sap.acacoop.com.ar/sap/opu/odata/SAP/API_BILL_OF_MATERIAL_SRV/" +
                                        $"A_BillOfMaterial(guid'{billOfMaterialHeaderUUID}')/to_BillOfMaterialItem?$format=json";
            var billOfMaterialItemDataDto = await _sapService.GetFromSapAsync<BillOfMaterialItemDataDto>(billOfMaterialItemUrl);

            if (billOfMaterialItemDataDto?.Results == null || billOfMaterialItemDataDto.Results.Count == 0)
            {
                await _logger.LogErrorAsync($"No existen RecipeBoms en la consulta a SAP", "Metodo: GetRecipeToAddAsync");
                return null;
            }

            recipe.RecipeBoms = billOfMaterialItemDataDto.Results
                .Where(r => !string.IsNullOrWhiteSpace(r.BillOfMaterialComponent))
                .Select(r => new RecipeBom
                {
                    BillOfMaterialItemUuid = Guid.TryParse(r.BillOfMaterialItemUUID, out var itemGuid) ? itemGuid : Guid.Empty,
                    BillOfMaterialHeaderUuid = billOfMaterialHeaderUUID,
                    BillOfMaterialComponent = r.BillOfMaterialComponent,
                    BillOfMaterialItemQuantity = ConverTo.FormatDecimal(r.BillOfMaterialItemQuantity)
                })
                .ToList();

            return recipe;
        }
        catch (Exception ex)
        {
            await _logger.LogErrorAsync(ex.ToString(), "Metodo: GetRecipeToAddAsync");
            throw;
        }

    }

    public async Task<BillOfMaterialHeaderDto> GetBillOfMaterialHeader(RecipeData data)
    {
        try
        {
            // Consulta a SAP por el dto que nos trae los datos material(producto) y planta
            var masterRecipeUrl = $"https://sapfioriqas.sap.acacoop.com.ar/sap/opu/odata/SAP/API_MASTER_RECIPE/MasterRecipeHeader(MasterRecipeGroup=" +
                    $"'{data.MasterRecipeGroup}',MasterRecipe='{data.MasterRecipe}',MasterRecipeInternalVersion='{data.MasterRecipeInternalVersion}')/to_MatlAssgmt?$format=json";
            var masterRecipeDto = await _sapService.GetFromSapAsync<MasterRecipeMatlAssgmtDto>(masterRecipeUrl);

            // Si no existe un producto o material, retorna null
            var first = masterRecipeDto.Results?.FirstOrDefault(r => !string.IsNullOrWhiteSpace(r.Product));
            if (first == null)
            {
                await _logger.LogErrorAsync($"No existe receta en la consulta a SAP con los siguientes datos: " +
                    $"MasterRecipeGroup: {data.MasterRecipeGroup}, MasterRecipe: {data.MasterRecipe}, MasterRecipeInternalVersion: {data.MasterRecipeInternalVersion}",
                    "Metodo: GetBillOfMaterialHeader, Parametro: RecipeData");
                return null;
            }

            // Consulta a SAP para traer la receta
            var billOfMaterialHeaderUrl = $"https://sapfioriqas.sap.acacoop.com.ar/sap/opu/odata/SAP/API_BILL_OF_MATERIAL_SRV/A_BillOfMaterial" +
                      $"?$filter=Material eq '{first.Product}' and Plant eq '{first.Plant}'" +
                      $"&$expand=to_BillOfMaterialItem&$format=json";
            var billOfMaterialHeaderDto = await _sapService.GetFromSapAsync<BillOfMaterialHeaderDto>(billOfMaterialHeaderUrl);

            if (billOfMaterialHeaderDto == null)
            {
                await _logger.LogErrorAsync($"No existe billOfMaterialHeader en la consulta a SAP con los siguientes datos: " +
                    $"material: {first.Product} planta: {first.Plant}", "Metodo: GetBillOfMaterialHeader, Parametro: RecipeData");
                return null;
            }

            // Retorna el dto que trae la receta y los datos necesarios para consultar los componentes o items de la receta
            return billOfMaterialHeaderDto;
        }
        catch (Exception ex)
        {
            await _logger.LogErrorAsync(ex.ToString(), "Metodo: GetBillOfMaterialHeader");
            throw;
        }

    }

    public async Task<BillOfMaterialHeaderDto> GetBillOfMaterialHeader(string material, string plant)
    {
        try
        {
            // Consulta a SAP para traer la receta
            var billOfMaterialHeaderUrl = $"https://sapfioriqas.sap.acacoop.com.ar/sap/opu/odata/SAP/API_BILL_OF_MATERIAL_SRV/A_BillOfMaterial" +
                      $"?$filter=Material eq '{material}' and Plant eq '{plant}'" +
                      $"&$expand=to_BillOfMaterialItem&$format=json";
            var billOfMaterialHeaderDto = await _sapService.GetFromSapAsync<BillOfMaterialHeaderDto>(billOfMaterialHeaderUrl);

            if (billOfMaterialHeaderDto == null)
            {
                await _logger.LogErrorAsync($"No existe billOfMaterialHeader en la consulta a SAP con los siguientes datos: " +
                    $"material: {material} planta: {plant}", "Metodo: GetBillOfMaterialHeader, Parametros: material, plant");
                return null;
            }

            // Retorna el dto que trae la receta y los datos necesarios para consultar los componentes o items de la receta
            return billOfMaterialHeaderDto;
        }
        catch (Exception ex)
        {
            await _logger.LogErrorAsync(ex.ToString(), "Metodo: GetBillOfMaterialHeader");
            throw;
        }

    }

    public async Task<List<Product>> GetProductsToAddAsync(List<string> materials)
    {
        try
        {
            var products = new List<Product>();

            foreach (var material in materials)
            {
                var product = await GetProductToAddAsync(material);
                products.Add(product); // Productos faltantes por ingresar en la tabla Product
            }

            return products;
        }
        catch (Exception ex)
        {
            await _logger.LogErrorAsync(ex.ToString(), "Metodo: GetProductsToAddAsync");
            throw;
        }

    }

    public async Task<Product> GetProductToAddAsync(string material)
    {
        try
        {
            // Consulta a SAP el producto
            string baseUrl = "https://sapfioriqas.sap.acacoop.com.ar:443/sap/opu/odata/sap/api_product_srv";
            string productUrl = $"{baseUrl}/A_Product('{material}')?$format=json";
            var productDto = await _sapService.GetFromSapAsync<ProductDto>(productUrl);

            if (productDto == null)
            {
                await _logger.LogErrorAsync($"No existe producto en la consulta a SAP con los siguientes datos: " +
                    $"material: {material}", "Metodo: GetProductToAddAsync");
                return null;
            }

            // Consulta a SAP la descripcion del producto
            string descriptionUrl = $"{baseUrl}/A_Product('{material}')/to_Description?$format=json";
            var productDescriptionDto = await _sapService.GetFromSapAsync<ProductDescriptionDto>(descriptionUrl);

            // Esto intentará primero con "ES" y, si no encuentra, tomará el primero disponible
            var productDescription = productDescriptionDto.Results?
                .FirstOrDefault(r => r.Language == "ES")?.ProductDescription
                ?? productDescriptionDto.Results?.FirstOrDefault()?.ProductDescription;

            if (productDescription == null)
            {
                await _logger.LogInfoAsync($"No existe descripcion del producto en la consulta a SAP con los siguientes datos: " +
                    $"material: {material}", "Metodo: GetProductToAddAsync");
            }

            var product = new Product
            {
                ProductCode = productDto.Product,
                ProductDescription = productDescription,
                ProductType = productDto.ProductType,
                CommStatus = 1,
                InterfaceCreateTimestamp = DateTime.Now
            };

            return product;
        }
        catch (Exception ex)
        {
            await _logger.LogErrorAsync(ex.ToString(), "Metodo: GetProductsToAddAsync");
            throw;
        }

    }
}
