using Domain.Dtos;
using Domain.Entities;
using Domain.Models;

namespace Application.Interfaces.Common;

public interface ICommonService
{
    Task<Recipe> GetRecipeToAddAsync(BillOfMaterialHeaderDto dto);
    Task<BillOfMaterialHeaderDto> GetBillOfMaterialHeader(RecipeData data);
    Task<BillOfMaterialHeaderDto> GetBillOfMaterialHeader(string material, string plant);
    Task<List<Product>> GetProductsToAddAsync(List<string> materials);
    Task<Product> GetProductToAddAsync(string material);
}
