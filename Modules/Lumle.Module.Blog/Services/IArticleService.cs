using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Lumle.Module.Blog.Entities;
using System.Linq;

namespace Lumle.Module.Blog.Services
{
    public interface IArticleService
    {
        int Count();
        int Count(Expression<Func<Article, bool>> predicate);
        IEnumerable<Article> GetAll(Expression<Func<Article, bool>> predicate);
        IQueryable<Article> GetAll();
        Article GetSingle(Expression<Func<Article, bool>> predicate);
        void Add(Article entity);
        void Update(Article entity);
        void DeleteWhere(Expression<Func<Article, bool>> predicate);
    }
}
