using API.Models;

namespace API.Data
{
  public interface IUserRepository
  {
    public bool SaveChanges();

    public void AddEntity<T>(T entityToAdd);

    public void RemoveEntity<T>(T entityToRemove);

    public IEnumerable<User> GetUsers();

    public User GetSingleUser(int userId);

    public UserJobInfo GetSingleUserJobInfo(int userId);

    public UserSalary GetSingleUserSalary(int userId);
  }
}