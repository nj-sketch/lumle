using Lumle.Data.Data.Abstracts;
using Lumle.Infrastructure.Constants.Log;
using Lumle.Module.Blog.Entities;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

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
                throw new Exception(ex.Message);
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
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<Article> GetAll(Expression<Func<Article, bool>> predicate)
        {
            try
            {
                var articles = _articleRepository.GetAll(predicate);
                return articles;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.DataFetchError);
                throw new Exception(ex.Message);
            }
        }

        public IQueryable<Article> GetAll()
        {
            try
            {
                var articles = _articleRepository.GetAll();
                return articles;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.DataFetchError);
                throw new Exception(ex.Message);
            }
        }

        public Article GetSingle(Expression<Func<Article, bool>> predicate)
        {
            try
            {
                var article = _articleRepository.GetSingle(predicate);
                return article;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.DataFetchError);
                throw new Exception(ex.Message);
            }
        }

        public void Add(Article entity)
        {
            try
            {
                _articleRepository.Add(entity);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.SaveError);
                throw new Exception(ex.Message);
            }
        }

        public void Update(Article entity)
        {
            try
            {
                _articleRepository.Update(entity);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.UpdateError);
                throw new Exception(ex.Message);
            }
        }

        public void DeleteWhere(Expression<Func<Article, bool>> predicate)
        {
            try
            {
                _articleRepository.DeleteWhere(predicate);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.DeleteError);
                throw new Exception(ex.Message);
            }
        }
    }
}