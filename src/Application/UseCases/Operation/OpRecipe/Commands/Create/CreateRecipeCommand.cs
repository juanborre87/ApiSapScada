using Application.Interfaces;
using Application.Interfaces.Common;
using Arq.Core;
using Arq.Host;
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
    ICommonService commonService,
    IFileLogger logger,
    IUnitOfWork uow)
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
            var billOfMaterialHeaderDto = await commonService.GetBillOfMaterialHeader(eventPayload.Data);
            if (billOfMaterialHeaderDto == null)
            {
                return new Response<CreateRecipeResponse>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new CreateRecipeResponse { Result = false, Message = "No existe material(producto) en la consulta a SAP" }
                };
            }


            // Consulta a SAP para traer recipe y recipeBoms
            var recipe = await commonService.GetRecipeToAddAsync(billOfMaterialHeaderDto);
            if (recipe == null)
            {
                return new Response<CreateRecipeResponse>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new CreateRecipeResponse { Result = false, Message = "No existe receta o recipeBoms en la consulta a SAP" }
                };
            }


            var recipeExist = await recipeQuery.FirstOrDefaultAsync(x => x.BillOfMaterialHeaderUuid == recipe.BillOfMaterialHeaderUuid, tracking: false);
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
            await logger.LogErrorAsync(ex.ToString(), "Metodo: CreateRecipeCommandHandler");
            return new Response<CreateRecipeResponse>
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Content = new CreateRecipeResponse { Result = false, Message = ex.Message }
            };
        }

    }
}
