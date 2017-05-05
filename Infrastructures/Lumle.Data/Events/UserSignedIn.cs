using MediatR;

namespace Lumle.Data.Events
{
    public class UserSignedIn : INotification
    {
        public string UserId { get; set; }
    }
}
