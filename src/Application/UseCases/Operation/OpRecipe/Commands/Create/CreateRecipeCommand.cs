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
            // Consulta a SAP para traer material y planta
            var billOfMaterialHeaderDto = await GetBillOfMaterialHeader(eventPayload.Data);
            if (billOfMaterialHeaderDto == null)
            {
                return new Response<CreateRecipeResponse>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new CreateRecipeResponse { Result = false, Message = "No existe material(producto) en la consulta a SAP" }
                };
            }


            // Consulta a SAP para traer recipe y recipeBoms
            var recipe = await GetRecipeToAddAsync(billOfMaterialHeaderDto);
            if (recipe == null)
            {
                return new Response<CreateRecipeResponse>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new CreateRecipeResponse { Result = false, Message = "No existe receta en la consulta a SAP" }
                };
            }


            var recipeExist = await recipeQuery.FirstOrDefaultAsync(x => x.BillOfMaterialHeaderUuid == recipe.BillOfMaterialHeaderUuid, false);
            if (recipeExist != null)
            {
                await logger.LogErrorAsync($"La receta ya existe, no se puede crear con el mismo nombre", "Metodo: CreateRecipeCommandHandler");
                return new Response<CreateRecipeResponse>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new CreateRecipeResponse { Result = false, Message = "La receta ya existe, no se puede crear con el mismo nombre" }
                };
            }

            // Adicion de recipe y recipeBoms
            await recipeCommand.AddAsync(recipe);
            await recipeBomCommand.AddRangeAsync(recipe.RecipeBoms);

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

    private async Task<BillOfMaterialHeaderDto> GetBillOfMaterialHeader(RecipeData data)
    {
        try
        {
            // Consulta a SAP por el dto que nos trae los datos material(producto) y planta
            var masterRecipeUrl = $"https://sapfioriqas.sap.acacoop.com.ar/sap/opu/odata/SAP/API_MASTER_RECIPE/MasterRecipeHeader(MasterRecipeGroup=" +
                    $"'{data.MasterRecipeGroup}',MasterRecipe='{data.MasterRecipe}',MasterRecipeInternalVersion='{data.MasterRecipeInternalVersion}')/to_MatlAssgmt?$format=json";
            var masterRecipeDto = await sapOrderService.GetFromSapAsync<MasterRecipeMatlAssgmtDto>(masterRecipeUrl);

            // Si no existe un producto o material, retorna null
            var first = masterRecipeDto.Results?.FirstOrDefault(r => !string.IsNullOrWhiteSpace(r.Product));
            if (first == null)
            {
                await logger.LogErrorAsync($"No existe material(producto) en la consulta a SAP", "Metodo: GetBillOfMaterialHeader");
                return null;
            }

            var billOfMaterialHeaderUrl = $"https://sapfioriqas.sap.acacoop.com.ar/sap/opu/odata/SAP/API_BILL_OF_MATERIAL_SRV/A_BillOfMaterial" +
                      $"?$filter=Material eq '{first.Product}' and Plant eq '{first.Plant}'" +
                      $"&$expand=to_BillOfMaterialItem&$format=json";
            var billOfMaterialHeaderDto = await sapOrderService.GetFromSapAsync<BillOfMaterialHeaderDto>(billOfMaterialHeaderUrl);

            // Retorna el dto que trae la receta y los datos necesarios para consultar los componentes o items de la receta
            return billOfMaterialHeaderDto;
        }
        catch (Exception ex)
        {
            await logger.LogErrorAsync(ex.Message.ToString(), "Metodo: GetBillOfMaterialHeader");
            throw;
        }

    }

    private async Task<Recipe> GetRecipeToAddAsync(BillOfMaterialHeaderDto dto)
    {
        try
        {
            // Si no existe una receta, retorna null
            var first = dto?.Results?.FirstOrDefault(r => !string.IsNullOrWhiteSpace(r.Material));
            if (first == null)
            {
                await logger.LogErrorAsync($"No existe receta en la consulta a SAP", "Metodo: GetRecipeToAddAsync");
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
            var billOfMaterialItemDataDto = await sapOrderService.GetFromSapAsync<BillOfMaterialItemDataDto>(billOfMaterialItemUrl);

            if (billOfMaterialItemDataDto?.Results == null || billOfMaterialItemDataDto.Results.Count == 0)
                recipe.RecipeBoms = [];

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
            await logger.LogErrorAsync(ex.Message.ToString(), "Metodo: GetRecipeToAddAsync");
            throw;
        }

    }

}
