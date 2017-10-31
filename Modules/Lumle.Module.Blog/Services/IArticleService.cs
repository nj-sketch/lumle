using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Lumle.Module.Blog.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace Lumle.Module.Blog.Services
{
    public interface IArticleService
    {
        int Count();
        int Count(Expression<Func<Article, bool>> predicate);
        IQueryable<Article> GetAll(Expression<Func<Article, bool>> predicate);
        IQueryable<Article> GetAll();
        Task<Article> GetSingle(Expression<Func<Article, bool>> predicate);
        Task Add(Article entity);
        Task Update(Article entity);
        Task DeleteWhere(Expression<Func<Article, bool>> predicate);
    }
}
