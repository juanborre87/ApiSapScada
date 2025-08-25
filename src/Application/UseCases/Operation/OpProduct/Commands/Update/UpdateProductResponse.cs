using Arq.Host;

namespace Application.UseCases.Operation.OpProduct.Commands.Update;

public class UpdateProductResponse : Notify
{
    public bool Result { get; set; }
}
