namespace Cinnamon.Application.Interfaces;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync();
}
