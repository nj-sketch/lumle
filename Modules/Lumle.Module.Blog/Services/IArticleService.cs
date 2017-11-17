using System;
using System.Linq.Expressions;
using Lumle.Module.Blog.Entities;
using System.Linq;
using System.Threading.Tasks;
using Lumle.Module.Blog.Models;
using static Lumle.Infrastructure.Helpers.DataTableHelper;
using Lumle.Data.Models;
using Lumle.Module.Blog.Helpers;
using Lumle.Module.Blog.ViewModels;

namespace Lumle.Module.Blog.Services
{
    public interface IArticleService
    {
        void DeleteWhere(Expression<Func<Article, bool>> predicate);
        Task Create(ArticleVM model, User loggedUser);
        Task Update(ArticleVM model, User loggedUser);
        Task<Article> GetSingleAsync(Expression<Func<Article, bool>> predicate);
        DTResult<ArticleModel> GetDataTableResult(User loggedUser, BlogDTParameters parameters);
    }
}
