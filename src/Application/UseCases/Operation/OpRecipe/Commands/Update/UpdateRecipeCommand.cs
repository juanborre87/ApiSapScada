using Application.Helpers;
using Application.Interfaces;
using Arq.Core;
using Arq.Host;
using Domain.Dtos;
using Domain.Entities;
using Domain.Models;
using Domain.Models.Payload;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace Application.UseCases.Operation.OpRecipe.Commands.Update;

public class UpdateRecipeCommand<T> : IRequest<Response<UpdateRecipeResponse>>
{
    public EventPayload<T> EventPayload { get; set; }
}

public class UpdateRecipeCommandHandler(
    IConfiguration configuration,
    IFileLogger logger,
    IUnitOfWork uow,
    ISapService sapOrderService)
    : IRequestHandler<UpdateRecipeCommand<RecipeData>, Response<UpdateRecipeResponse>>
{
    public async Task<Response<UpdateRecipeResponse>> Handle(UpdateRecipeCommand<RecipeData> request, CancellationToken cancellationToken)
    {
        var eventPayload = request.EventPayload;
        if (eventPayload == null)
        {
            await logger.LogInfoAsync("El request es inválido", "Metodo: UpdateRecipeCommandHandler");
            return new Response<UpdateRecipeResponse>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new UpdateRecipeResponse { Result = false, Message = "El request es inválido" }
            };
        }

        await logger.LogInfoAsync("Inicio de actualización de una receta", "Metodo: UpdateRecipeCommandHandler");
        await uow.BeginTransactionAsync("SapScada");

        var recipeCommand = uow.CommandRepository<Recipe>("SapScada");
        var recipeQuery = uow.QueryRepository<Recipe>("SapScada");
        var recipeBomCommand = uow.CommandRepository<RecipeBom>("SapScada");

        try
        {
            var billOfMaterialHeader = await GetBillOfMaterialHeader(eventPayload.Data.MasterRecipe, "5000");
            var recipeExist = await recipeQuery.FirstOrDefaultIncludeMultipleAsync(
                x => x.BillOfMaterialHeaderUuid == billOfMaterialHeader.Item2, 
                tracking: true,
                q => q.Include(x => x.RecipeBoms));
            if (recipeExist == null)
            {
                await logger.LogErrorAsync($"La receta no existe, no se puede actualizar", "Metodo: UpdateRecipeCommandHandler");
                return new Response<UpdateRecipeResponse>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new UpdateRecipeResponse { Result = false, Message = "La receta no existe, no se puede actualizar" }
                };
            }

            var recipe = GetRecipeToAddAsync(billOfMaterialHeader.Item1, billOfMaterialHeader.Item2);
            var recipesBom = await GetRecipesBomToAddAsync(billOfMaterialHeader.Item2);
            await recipeBomCommand.DeleteRangeAsync(recipeExist.RecipeBoms);
            recipeExist.InterfaceUpdateTimestamp = DateTime.Now;
            recipeExist.BillOfMaterial = recipe.BillOfMaterial;
            recipeExist.Material = recipe.Material;
            recipeExist.CommStatus = 1;
            await recipeCommand.UpdateAsync(recipeExist);
            await recipeBomCommand.AddRangeAsync(recipesBom);

            await logger.LogInfoAsync($"Los registros fueron creados con exito", "Metodo: UpdateRecipeCommandHandler");
            return new Response<UpdateRecipeResponse>
            {
                StatusCode = HttpStatusCode.OK,
                Content = new UpdateRecipeResponse { Result = true, Message = "Los registros fueron creados con exito" }
            };
        }
        catch (Exception ex)
        {
            await uow.RollbackAsync("SapScada");
            await logger.LogErrorAsync(ex.Message.ToString(), "Metodo: UpdateRecipeCommandHandler");
            return new Response<UpdateRecipeResponse>
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Content = new UpdateRecipeResponse { Result = false, Message = ex.Message }
            };
        }

    }

    private async Task<(BillOfMaterialHeaderDto, Guid)> GetBillOfMaterialHeader(string material, string plant)
    {
        try
        {
            // Consulta a SAP
            var billOfMaterialHeaderUrl = $"https://sapfioriqas.sap.acacoop.com.ar/sap/opu/odata/SAP/API_BILL_OF_MATERIAL_SRV/A_BillOfMaterial" +
                      $"?$filter=Material eq '{material}' and Plant eq '{plant}'" +
                      $"&$expand=to_BillOfMaterialItem&$format=json";
            var billOfMaterialHeaderDto = await sapOrderService.GetFromSapAsync<BillOfMaterialHeaderDto>(billOfMaterialHeaderUrl);

            var uuidString = billOfMaterialHeaderDto.Results?.FirstOrDefault()?.BillOfMaterialHeaderUUID;

            if (Guid.TryParse(uuidString, out var guidValue))
                return (billOfMaterialHeaderDto, guidValue);

            return (billOfMaterialHeaderDto, Guid.Empty);
        }
        catch (Exception ex)
        {
            await logger.LogErrorAsync(ex.Message.ToString(), "Metodo: GetBillOfMaterialHeader");
            throw;
        }

    }

    private static Recipe GetRecipeToAddAsync(BillOfMaterialHeaderDto dto, Guid billOfMaterialHeaderUUID)
    {
        var first = dto?.Results?
                      .FirstOrDefault(r => !string.IsNullOrWhiteSpace(r.Material));

        if (first is null)
            return null;

        return new Recipe
        {
            BillOfMaterialHeaderUuid = billOfMaterialHeaderUUID,
            Material = first.Material,
            BillOfMaterial = first.BillOfMaterial,
            InterfaceCreateTimestamp = DateTime.Now,
            CommStatus = 1
        };
    }

    private async Task<List<RecipeBom>> GetRecipesBomToAddAsync(Guid billOfMaterialHeaderUUID)
    {
        try
        {
            // Consulta a SAP
            var billOfMaterialItemUrl = $"https://sapfioriqas.sap.acacoop.com.ar/sap/opu/odata/SAP/API_BILL_OF_MATERIAL_SRV/" +
                                        $"A_BillOfMaterial(guid'{billOfMaterialHeaderUUID}')/to_BillOfMaterialItem?$format=json";
            var billOfMaterialItemDataDto = await sapOrderService.GetFromSapAsync<BillOfMaterialItemDataDto>(billOfMaterialItemUrl);

            if (billOfMaterialItemDataDto?.Results == null || billOfMaterialItemDataDto.Results.Count == 0)
                return [];

            var recipesBom = billOfMaterialItemDataDto.Results
                .Where(r => !string.IsNullOrWhiteSpace(r.BillOfMaterialComponent))
                .Select(r => new RecipeBom
                {
                    BillOfMaterialItemUuid = Guid.TryParse(r.BillOfMaterialItemUUID, out var itemGuid) ? itemGuid : Guid.Empty,
                    BillOfMaterialHeaderUuid = billOfMaterialHeaderUUID,
                    BillOfMaterialComponent = r.BillOfMaterialComponent,
                    BillOfMaterialItemQuantity = ConverTo.FormatDecimal(r.BillOfMaterialItemQuantity)
                })
                .ToList();

            return recipesBom;
        }
        catch (Exception ex)
        {
            await logger.LogErrorAsync(ex.Message.ToString(), "Metodo: GetRecipeBOMToAddAsync");
            throw;
        }

    }

}
