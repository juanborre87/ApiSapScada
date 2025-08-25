using Arq.Host;

namespace Application.UseCases.Operation.OpRecipe.Commands.Update;

public class UpdateRecipeResponse : Notify
{
    public bool Result { get; set; }
}
