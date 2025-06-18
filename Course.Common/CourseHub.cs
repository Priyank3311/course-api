using Microsoft.AspNetCore.SignalR;

namespace Course.Common;

public class CourseHub : Hub
{
    public string GetConnectionId() => Context.ConnectionId;

    public async Task NotifyNewCourse()
    {
        string connectionId = GetConnectionId();
        await Clients.AllExcept(connectionId).SendAsync("NewCourseAdded");
    }

    public async Task RefreshStudentCourses()
    {
        string connectionId = GetConnectionId();
        await Clients.AllExcept(connectionId).SendAsync("RefreshStudentCourses");
    }

    //  public async Task NotifyCourseUpdated(CourseResponseDto updatedCourse)
    // {
    //     await Clients.All.SendAsync("CourseUpdated", updatedCourse);
    // }

    // public async Task NotifyCourseDeleted(int courseId)
    // {
    //     await Clients.All.SendAsync("CourseDeleted", courseId);
    // }
}

