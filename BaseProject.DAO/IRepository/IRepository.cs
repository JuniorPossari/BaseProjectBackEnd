using System.Linq.Expressions;
using BaseProject.DAO.Data;

namespace BaseProject.DAO.IRepository
{
	public interface IRepository<TEntity>
    {
        bool Insert(TEntity entity);

        bool InsertAll(IEnumerable<TEntity> entities);

        bool Delete(int id);

        bool Delete(TEntity entity);

		bool DeleteAll(IEnumerable<TEntity> entities);

        bool DeleteByFilter(Expression<Func<TEntity, bool>> filter);

        bool Update(TEntity entity);

        bool UpdateAll(IEnumerable<TEntity> entities);

        T[] Select<T>(
            Expression<Func<TEntity, T>> keySelector,
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "",
            bool noTracking = false
        );

        TEntity FirstOrDefault(
            Expression<Func<TEntity, bool>> filter,
            string includeProperties = "",
            bool noTracking = false,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null
        );

        bool Exists(Expression<Func<TEntity, bool>> filter);

        TEntity[] Get(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "",
            bool noTracking = false);

        ApplicationDbContext GetContext();

        TEntity[] Filter(
            IQueryable<TEntity> query,
            int initialPosition,
            int itensPerPage,
            out int total,
            string includeProperties = ""
        );

        T[] FilterSelect<T>(
            Expression<Func<TEntity, T>> keySelector,
            IQueryable<TEntity> query,
            int initialPosition,
            int itensPerPage,
            out int total,
            string includeProperties = ""
        );
    }
}
