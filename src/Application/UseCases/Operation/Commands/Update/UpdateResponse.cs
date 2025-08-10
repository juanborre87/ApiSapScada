using Arq.Host;

namespace Application.UseCases.Operation.Commands.Update
{
    public class UpdateResponse : Notify
    {
        public bool Result { get; set; }
    }
}
