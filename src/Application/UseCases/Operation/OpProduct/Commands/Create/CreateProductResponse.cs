using Arq.Host;

namespace Application.UseCases.Operation.OpProduct.Commands.Create;

public class CreateProductResponse : Notify
{
    public bool Result { get; set; }
}
