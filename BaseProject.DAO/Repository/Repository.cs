using Microsoft.EntityFrameworkCore;
using BaseProject.DAO.IRepository;
using System.Linq.Expressions;
using BaseProject.DAO.Data;

namespace BaseProject.DAO.Repository
{
	public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
	{
		protected readonly ApplicationDbContext _context;
		private readonly char[] _separator = [','];

		public Repository(ApplicationDbContext context)
        {
            _context = context;
        }

        public virtual bool Insert(TEntity entity)
        {
            try
            {
                _context.Set<TEntity>().Add(entity);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }
        
        public virtual bool InsertAll(IEnumerable<TEntity> entities)
        {
            if (!entities.Any()) return true;

			try
            {
                for (int i = 0; i < entities.Count(); i++) _context.Entry(entities.ElementAt(i)).State = EntityState.Added;

				_context.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
				Console.WriteLine(ex.ToString());
				return false;
            }
        }

        public virtual bool Delete(int id)
        {
            if (id == 0) return true;

            var entity = _context.Set<TEntity>().Find(id);

            return Delete(entity);
        }

        public virtual bool DeleteByFilter(Expression<Func<TEntity, bool>> filter)
        {
            var toDelete = _context.Set<TEntity>().Where(filter).ToArray();

            try
            {
                _context.Set<TEntity>().RemoveRange(toDelete);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
				Console.WriteLine(ex.ToString());
				return false;
            }
        }
        
        public virtual bool Delete(TEntity entity)
        {
            if (_context.Entry(entity).State == EntityState.Detached) _context.Set<TEntity>().Attach(entity);

			try
            {
                _context.Set<TEntity>().Remove(entity);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
				Console.WriteLine(ex.ToString());
				return false;
            }
        }

        public virtual TEntity[] Filter(
            IQueryable<TEntity> query,
            int initialPosition,
            int itensPerPage,
            out int total,
            string includeProperties = ""
        )
        {
			foreach (var includeProperty in includeProperties.Split(_separator, StringSplitOptions.RemoveEmptyEntries)) query = query.Include(includeProperty);

			total = query.Count();
            query = query.AsNoTracking();
            query = query.Skip(initialPosition);
            query = query.Take(itensPerPage);

            return query.ToArray();
        }

        public virtual T[] FilterSelect<T>(
            Expression<Func<TEntity, T>> keySelector,
            IQueryable<TEntity> query,
            int initialPosition,
            int itensPerPage,
            out int total,
            string includeProperties = ""
        )
        {
			foreach (var includeProperty in includeProperties.Split(_separator, StringSplitOptions.RemoveEmptyEntries)) query = query.Include(includeProperty);

			query = query.AsNoTracking();
            total = query.Count();
            query = query.Skip(initialPosition);
            query = query.Take(itensPerPage);

            return query.Select(keySelector).ToArray();
        }

        public virtual bool DeleteAll(IEnumerable<TEntity> entities)
        {
            if (entities.Count() == 0) return true;

			int entityCount = entities.Count();

            for (int i = 0; i < entityCount; i++) if (_context.Entry(entities.ElementAt(i)).State == EntityState.Detached) _context.Set<TEntity>().Attach(entities.ElementAt(i));

			_context.Set<TEntity>().RemoveRange(entities);

            try
            {
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
				Console.WriteLine(ex.ToString());
				return false;
            }
        }

        public virtual bool Update(TEntity entity)
        {
            try
            {
                _context.Entry(entity).State = EntityState.Modified;
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
				Console.WriteLine(ex.ToString());
				return false;
            }
        }

        public virtual bool UpdateAll(IEnumerable<TEntity> entities)
        {
            if (!entities.Any()) return true;

			try
            {
                for (int i = 0; i < entities.Count(); i++) _context.Entry(entities.ElementAt(i)).State = EntityState.Modified;

				_context.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
				Console.WriteLine(ex.ToString());
				return false;
            }
        }

        public virtual T[] Select<T>(
            Expression<Func<TEntity, T>> keySelector,
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "",
            bool noTracking = false
        )
        {
            var query = _context.Set<TEntity>().AsQueryable();

            if (filter != null) query = query.Where(filter);

			foreach (var includeProperty in includeProperties.Split(_separator, StringSplitOptions.RemoveEmptyEntries)) query = query.Include(includeProperty);

			if (noTracking) query = query.AsNoTracking();

			if (orderBy != null) query = orderBy(query);

			var selectedQuery = query.Select(keySelector);

            return selectedQuery.ToArray();
        }

        public virtual TEntity FirstOrDefault(
            Expression<Func<TEntity, bool>> filter,
            string includeProperties = "",
            bool noTracking = false,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null
        )
        {
            var query = _context.Set<TEntity>().AsQueryable();

			foreach (var includeProperty in includeProperties.Split(_separator, StringSplitOptions.RemoveEmptyEntries)) query = query.Include(includeProperty);

			if (noTracking) query = query.AsNoTracking();

			if (orderBy != null) query = orderBy(query);

			return query.FirstOrDefault(filter);
        }

        public virtual bool Exists(Expression<Func<TEntity, bool>> filter)
        {
            var query = _context.Set<TEntity>().AsQueryable();

            query = query.AsNoTracking();

            return query.Any(filter);
        }

        public virtual TEntity[] Get(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "",
            bool noTracking = false)
        {
            var query = _context.Set<TEntity>().AsQueryable();

            if (filter != null) query = query.Where(filter);

			if (orderBy != null) query = orderBy(query);

			foreach (var includeProperty in includeProperties.Split(_separator, StringSplitOptions.RemoveEmptyEntries)) query = query.Include(includeProperty);

			if (noTracking) query = query.AsNoTracking();

			return query.ToArray();
        }

        public ApplicationDbContext GetContext()
        {
            return _context;
        }
    }
}
