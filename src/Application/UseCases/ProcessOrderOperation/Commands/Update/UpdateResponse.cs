using Arq.Host;

namespace Application.UseCases.ProcessOrderOperation.Commands.Update;

public class UpdateResponse : Notify
{
    public bool Result { get; set; }
}
