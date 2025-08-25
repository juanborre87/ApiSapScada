using Arq.Host;

namespace Application.UseCases.Operation.OpOrder.Commands.Create;

public class CreateOrderResponse : Notify
{
    public bool Result { get; set; }
}
