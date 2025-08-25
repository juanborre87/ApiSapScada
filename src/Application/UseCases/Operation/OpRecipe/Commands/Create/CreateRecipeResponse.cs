using Arq.Host;

namespace Application.UseCases.Operation.OpRecipe.Commands.Create;

public class CreateRecipeResponse : Notify
{
    public bool Result { get; set; }
}