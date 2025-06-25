using HostWorker.Models;

namespace Application.UseCases.Operation.Commands.Create
{
    public class CreateResponse : Notify
    {
        public bool Result { get; set; }
    }
}
