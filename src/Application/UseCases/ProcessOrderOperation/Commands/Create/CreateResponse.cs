using Arq.Host;

namespace Application.UseCases.ProcessOrderOperation.Commands.Create;

public class CreateResponse : Notify
{
    public bool Result { get; set; }
}
