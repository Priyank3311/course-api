namespace Course.Services.Common;

public interface IFirebaseNotificationService
{

    Task SendToTokenAsync(string token, string title, string body);
}