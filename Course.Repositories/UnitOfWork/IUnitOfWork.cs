using Course.DataModel.Entities;
using Course.Repositories.BaseRepository;

namespace Course.Repositories.UnitOfWork;

public interface IUnitOfWork
{
    public IBaseRepository<Course.DataModel.Entities.Course> Course { get; }
    public IBaseRepository<User> User { get; }
    public IBaseRepository<Enrollment> Enrollment { get; }
}
