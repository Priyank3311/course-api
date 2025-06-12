using Course.DataModel.Entities;
using Course.Repositories.BaseRepository;

namespace Course.Repositories.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly StudentCourseDbContext context;

    public UnitOfWork(StudentCourseDbContext _context)
    {
        context = _context;
        Course = new BaseRepository<Course.DataModel.Entities.Course>(context);
        User = new BaseRepository<User>(context);
        Enrollment = new BaseRepository<Enrollment>(context);

    }

    public IBaseRepository<Course.DataModel.Entities.Course> Course { get; private set; }
    public IBaseRepository<User> User { get; private set; }
    public IBaseRepository<Enrollment> Enrollment { get; private set; }
}
