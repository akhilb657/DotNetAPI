namespace API.Data
{
  public interface IUserRepository
  {
    public bool SaveChanges();

    public void AddEntity<T>(T entityToAdd);

    public void RemoveEntity<T>(T entityToRemove);
  }
}