using Application.Interfaces;
using Application.Interfaces.Common;
using Arq.Core;
using Arq.Host;
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
    ICommonService commonService,
    IFileLogger logger,
    IUnitOfWork uow)
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
            // Consulta a SAP para traer material y planta
            var billOfMaterialHeaderDto = await commonService.GetBillOfMaterialHeader(eventPayload.Data);
            if (billOfMaterialHeaderDto == null)
            {
                return new Response<UpdateRecipeResponse>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new UpdateRecipeResponse { Result = false, Message = "No existe material(producto) en la consulta a SAP" }
                };
            }


            // Consulta a SAP para traer recipe y recipeBoms
            var recipe = await commonService.GetRecipeToAddAsync(billOfMaterialHeaderDto);
            if (recipe == null)
            {
                return new Response<UpdateRecipeResponse>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new UpdateRecipeResponse { Result = false, Message = "No existe receta o recipeBoms en la consulta a SAP" }
                };
            }


            var recipeExist = await recipeQuery.FirstOrDefaultIncludeMultipleAsync(
                x => x.BillOfMaterialHeaderUuid == recipe.BillOfMaterialHeaderUuid,
                tracking: true,
                q => q.Include(x => x.RecipeBoms));
            if (recipeExist == null)
            {
                await logger.LogErrorAsync($"La receta no existe en la Bd, no se puede actualizar", "Metodo: UpdateRecipeCommandHandler");
                return new Response<UpdateRecipeResponse>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new UpdateRecipeResponse { Result = false, Message = "La receta no existe, no se puede actualizar" }
                };
            }

            await recipeBomCommand.DeleteRangeAsync(recipeExist.RecipeBoms);
            recipeExist.InterfaceUpdateTimestamp = DateTime.Now;
            recipeExist.BillOfMaterial = recipe.BillOfMaterial;
            recipeExist.Material = recipe.Material;
            recipeExist.CommStatus = 1;
            await recipeCommand.UpdateAsync(recipeExist);
            await recipeBomCommand.AddRangeAsync(recipe.RecipeBoms);

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
            await logger.LogErrorAsync(ex.ToString(), "Metodo: UpdateRecipeCommandHandler");
            return new Response<UpdateRecipeResponse>
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Content = new UpdateRecipeResponse { Result = false, Message = ex.Message }
            };
        }

    }

}
