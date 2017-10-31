using Lumle.Data.Data.Abstracts;
using Lumle.Infrastructure.Constants.LumleLog;
using Lumle.Module.Blog.Entities;
using NLog;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Lumle.Module.Blog.Services
{
    public class ArticleService : IArticleService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IRepository<Article> _articleRepository;

        public ArticleService(IRepository<Article> articleRepository)
        {
            _articleRepository = articleRepository;
        }

        public int Count()
        {
            try
            {
                var articles = _articleRepository.Count();
                return articles;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.DataFetchError);
                throw;
            }
        }

        public int Count(Expression<Func<Article, bool>> predicate)
        {
            try
            {
                var articles = _articleRepository.Count(predicate);
                return articles;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.DataFetchError);
                throw;
            }
        }

        public IQueryable<Article> GetAll(Expression<Func<Article, bool>> predicate)
        {
            try
            {
                var articles = _articleRepository.GetAll(predicate);
                return articles;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.DataFetchError);
                throw;
            }
        }

        public IQueryable<Article> GetAll()
        {
            try
            {
                var articles =  _articleRepository.GetAll();
                return articles;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.DataFetchError);
                throw;
            }
        }

        public async Task<Article> GetSingle(Expression<Func<Article, bool>> predicate)
        {
            try
            {
                var article = await _articleRepository.GetSingle(predicate);
                return article;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.DataFetchError);
                throw new Exception(ex.Message);
            }
        }

        public async Task Add(Article entity)
        {
            try
            {
               await _articleRepository.Add(entity);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.SaveError);
                throw;
            }
        }

        public async Task Update(Article entity)
        {
            try
            {
                await _articleRepository.Update(entity);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.UpdateError);
                throw;
            }
        }

        public async Task DeleteWhere(Expression<Func<Article, bool>> predicate)
        {
            try
            {
                await _articleRepository.DeleteWhere(predicate);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.DeleteError);
                throw;
            }
        }    
    }
}