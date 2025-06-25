using System.Net;

namespace HostWorker.Models;

public class Response<T>
{
    public Response()
    {
        Notifications = new List<Notify>();
        Headers = new Dictionary<string, string>();
    }

    public T? Content { get; set; }

    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;


    public List<Notify> Notifications { get; }

    public bool IsValid => Notifications.Count == 0;

    public Dictionary<string, string> Headers { get; set; }

    public void AddNotifications(IList<Notify> notifies)
    {
        Notifications.AddRange(notifies);
    }

    public void AddNotification(Notify notification)
    {
        Notifications.Add(notification);
    }

    public void AddNotification(bool isException, string property, string message)
    {
        Notifications.Add(new Notify { IsException = isException, Message = message, Property = property });
    }
}