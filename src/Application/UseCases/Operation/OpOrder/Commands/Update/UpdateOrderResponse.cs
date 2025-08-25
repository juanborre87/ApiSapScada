using Arq.Host;

namespace Application.UseCases.Operation.OpOrder.Commands.Update;

public class UpdateOrderResponse : Notify
{
    public bool Result { get; set; }
}
