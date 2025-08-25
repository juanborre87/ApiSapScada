using Application.Helpers;
using Application.Interfaces;
using Arq.Core;
using Arq.Host;
using Domain.Dtos;
using Domain.Entities;
using Domain.Models;
using Domain.Models.Payload;
using MediatR;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace Application.UseCases.Operation.OpRecipe.Commands.Create;

public class CreateRecipeCommand<T> : IRequest<Response<CreateRecipeResponse>>
{
    public EventPayload<T> EventPayload { get; set; }
}

public class CreateRecipeCommandHandler(
    IConfiguration configuration,
    IFileLogger logger,
    IUnitOfWork uow,
    ISapService sapOrderService)
    : IRequestHandler<CreateRecipeCommand<RecipeData>, Response<CreateRecipeResponse>>
{
    public async Task<Response<CreateRecipeResponse>> Handle(CreateRecipeCommand<RecipeData> request, CancellationToken cancellationToken)
    {
        var eventPayload = request.EventPayload;
        if (eventPayload == null)
        {
            await logger.LogInfoAsync("El request es inválido", "Metodo: CreateRecipeCommandHandler");
            return new Response<CreateRecipeResponse>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new CreateRecipeResponse { Result = false, Message = "El request es inválido" }
            };
        }

        await logger.LogInfoAsync("Inicio de creación de una nueva receta", "Metodo: CreateRecipeCommandHandler");
        await uow.BeginTransactionAsync("SapScada");

        var recipeCommand = uow.CommandRepository<Recipe>("SapScada");
        var recipeQuery = uow.QueryRepository<Recipe>("SapScada");
        var recipeBomCommand = uow.CommandRepository<RecipeBom>("SapScada");

        try
        {
            var billOfMaterialHeader = await GetBillOfMaterialHeader(eventPayload.Data.MasterRecipe, "5000");
            var recipeExist = await recipeQuery.FirstOrDefaultAsync(x => x.BillOfMaterialHeaderUuid == billOfMaterialHeader.Item2, false);
            if (recipeExist != null)
            {
                await logger.LogErrorAsync($"La receta ya existe, no se puede crear con el mismo nombre", "Metodo: CreateRecipeCommandHandler");
                return new Response<CreateRecipeResponse>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new CreateRecipeResponse { Result = false, Message = "La receta ya existe, no se puede crear con el mismo nombre" }
                };
            }

            var recipe = GetRecipeToAddAsync(billOfMaterialHeader.Item1, billOfMaterialHeader.Item2);
            var recipesBom = await GetRecipesBomToAddAsync(billOfMaterialHeader.Item2);
            await recipeCommand.AddAsync(recipe);
            await recipeBomCommand.AddRangeAsync(recipesBom);

            await logger.LogInfoAsync($"Los registros fueron creados con exito", "Metodo: CreateRecipeCommandHandler");
            return new Response<CreateRecipeResponse>
            {
                StatusCode = HttpStatusCode.OK,
                Content = new CreateRecipeResponse { Result = true, Message = "Los registros fueron creados con exito" }
            };
        }
        catch (Exception ex)
        {
            await uow.RollbackAsync("SapScada");
            await logger.LogErrorAsync(ex.Message.ToString(), "Metodo: CreateRecipeCommandHandler");
            return new Response<CreateRecipeResponse>
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Content = new CreateRecipeResponse { Result = false, Message = ex.Message }
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
                    BillOfMaterialItemQuantity = ConverTo.FormatFloat(r.BillOfMaterialItemQuantity)
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
