using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;

namespace Course.Services.Common;

public class FirebaseNotificationService : IFirebaseNotificationService
{
    private readonly FirebaseMessaging _messaging;

    public FirebaseNotificationService()
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "firebase-config.json");
        FirebaseApp.Create(new AppOptions()
        {
            Credential = GoogleCredential.FromFile(path)
        });

        _messaging = FirebaseMessaging.DefaultInstance;
    }

    public async Task SendToTokenAsync(string token, string title, string body)
    {
        var message = new Message()
        {
            Token = token,
            Notification = new Notification()
            {
                Title = title,
                Body = body
            }
        };

        await _messaging.SendAsync(message);
    }
}
