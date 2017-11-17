using Lumle.Data.Data.Abstracts;
using Lumle.Infrastructure.Constants.LumleLog;
using Lumle.Module.Blog.Entities;
using NLog;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Lumle.Data.Models;
using Lumle.Infrastructure.Helpers;
using Lumle.Module.Blog.Helpers;
using Lumle.Module.Blog.Models;
using System.Collections.Generic;
using Lumle.Infrastructure.Utilities.Abstracts;
using static Lumle.Infrastructure.Helpers.DataTableHelper;
using Lumle.Module.Audit.Services;
using Lumle.Module.Blog.ViewModels;
using AutoMapper;
using Lumle.Module.Audit.Models;
using Lumle.Module.Audit.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Lumle.Module.Blog.Services
{
    public class ArticleService : IArticleService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IRepository<Article> _articleRepository;
        private readonly ITimeZoneHelper _timeZoneHelper;
        private readonly IAuditLogService _auditLogService;
        private readonly IFileHandler _fileHandler;
        private string _imageUrl;

        public ArticleService(
            IRepository<Article> articleRepository,
            ITimeZoneHelper timeZoneHelper,
            IAuditLogService auditLogService,
            IFileHandler fileHandler,
            IUrlHelper urlHelper
        )
        {
            _articleRepository = articleRepository;
            _timeZoneHelper = timeZoneHelper;
            _auditLogService = auditLogService;
            _fileHandler = fileHandler;
        }

        public async Task<Article> GetSingleAsync(Expression<Func<Article, bool>> predicate)
        {
            try
            {
                var article = await _articleRepository.GetSingleAsync(predicate);
                return article;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.DataFetchError);
                throw;
            }
        }

        public async Task Create(ArticleVM model, User loggedUser)
        {
            try
            {
                if (model.FeaturedImage != null)
                {
                    _imageUrl = _fileHandler.UploadImage(model.FeaturedImage, 300, 300);
                }

                var articleModel = Mapper.Map<ArticleModel>(model);
                articleModel.Author = loggedUser.Email;
                articleModel.CreatedDate = DateTime.UtcNow;
                articleModel.LastUpdated = DateTime.UtcNow;
                articleModel.Slug = "Test slug";
                articleModel.FeaturedImageUrl = _imageUrl;

                // Check if the content field is null
                if (!String.IsNullOrEmpty(articleModel.Content))
                {
                    // Convert Base64 image to image format
                    articleModel.Content = await _fileHandler.SaveImageFromEditor(articleModel.Content, "blogs");
                }

                var articleEntity = Mapper.Map<Article>(articleModel);

                await _articleRepository.AddAsync(articleEntity);
                await _articleRepository.SaveChangesAsync();

                #region AuditLog
                var newArticle = new Article(); // Storage of this null object shows data before creation = nothing!
                var auditLogModel = new AuditLogModel
                {
                    AuditActionType = AuditActionType.Create,
                    OldObject = newArticle,
                    NewObject = articleEntity,
                    KeyField = articleEntity.Id.ToString(),
                    ComparisonType = ComparisonType.ObjectCompare,
                    LoggedUserEmail = loggedUser.Email
                };
                #endregion
                await _auditLogService.Create(auditLogModel);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.SaveError);
                throw;
            }
        }

        public async Task Update(ArticleVM model, User loggedUser)
        {
            try
            {
                // Save images if any
                if (model.FeaturedImage != null)
                {
                    _imageUrl = _fileHandler.UploadImage(model.FeaturedImage, 300, 360);
                }

                var oldArticle = await _articleRepository.GetSingleAsync(x => x.Id == model.Id);
                // Add previous data in old record object for comparison
                var oldRecord = new Article
                {
                    Id = oldArticle.Id,
                    Author = oldArticle.Author,
                    Title = oldArticle.Title,
                    Content = oldArticle.Content,
                    FeaturedImageUrl = oldArticle.FeaturedImageUrl,
                    Slug = oldArticle.Slug
                };

                // update in database
                oldArticle.Title = model.Title;

                // Check if content field is null or empty
                if (!String.IsNullOrEmpty(model.Content))
                {
                    oldArticle.Content = await _fileHandler.SaveImageFromEditor(model.Content, "blogs");
                }

                oldArticle.FeaturedImageUrl = _imageUrl ?? oldArticle.FeaturedImageUrl;

                await _articleRepository.UpdateAsync(oldArticle, oldArticle.Id);
                await _articleRepository.SaveChangesAsync();
                // For audit log
                #region AuditLog
                var auditLogModel = new AuditLogModel
                {
                    AuditActionType = AuditActionType.Update,
                    OldObject = oldRecord,
                    NewObject = oldArticle,
                    KeyField = oldRecord.Id.ToString(),
                    ComparisonType = ComparisonType.ObjectCompare,
                    LoggedUserEmail = loggedUser.Email
                };
                await _auditLogService.Create(auditLogModel);

                #endregion
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.DataFetchError);
                throw;
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
                throw;
            }
        }

        public DataTableHelper.DTResult<ArticleModel> GetDataTableResult(User loggedUser, BlogDTParameters parameters)
        {
            try
            {
                var articles = _articleRepository.GetAll().Select(art => new ArticleModel
                {
                    Id = art.Id,
                    Title = art.Title,
                    Author = art.Author,
                    CreatedDate = art.CreatedDate
                });

                List<ArticleModel> filteredAtricle;
                int totalArticle;
                if (!string.IsNullOrEmpty(parameters.Search.Value.Trim())
                   && !string.IsNullOrWhiteSpace(parameters.Search.Value.Trim()))
                {
                    Expression<Func<ArticleModel, bool>> search =
                        x => (x.Author ?? "").ToString().ToLower().Contains(parameters.Search.Value.ToLower()) ||
                             (x.Title ?? "").ToString().ToLower().Contains(parameters.Search.Value.ToLower());
                    totalArticle = articles.Count(search);
                    articles = articles.Where(search);
                    articles = SortByColumnWithOrder(parameters.Order[0].Column, parameters.Order[0].Dir.ToString(), articles);
                    filteredAtricle = articles.Skip(parameters.Start).Take(parameters.Length).ToList();
                }
                else
                {
                    totalArticle = articles.Count();
                    articles = SortByColumnWithOrder(parameters.Order[0].Column, parameters.Order[0].Dir.ToString(), articles);
                    filteredAtricle = articles.Skip(parameters.Start).Take(parameters.Length).ToList();
                }
                var counter = parameters.Start + 1;
                var articleModels = filteredAtricle.Select(x => {
                    x.SN = counter++;
                    x.FormatedCreatedDate = _timeZoneHelper.ConvertToLocalTime(x.CreatedDate, loggedUser.TimeZone).ToString("g");
                    return x;
                }).ToList();

                var datatableResult = new DTResult<ArticleModel>
                {
                    Draw = parameters.Draw,
                    Data = articleModels,
                    RecordsFiltered = totalArticle,
                    RecordsTotal = totalArticle
                };

                return datatableResult;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.DataFetchError);
                throw;
            }
        }

        #region Helpers
        private static IQueryable<ArticleModel> SortByColumnWithOrder(int order, string orderDirection, IQueryable<ArticleModel> data)
        {
            try
            {
                switch (order)
                {
                    case 2:
                        return orderDirection.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.Title) : data.OrderBy(p => p.Title);
                    case 3:
                        return orderDirection.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.Author) : data.OrderBy(p => p.Author);
                    case 4:
                        return orderDirection.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.CreatedDate) : data.OrderBy(p => p.CreatedDate);
                    default:
                        return data.OrderByDescending(p => p.CreatedDate);
                }
            }
            catch (Exception)
            {
                return data;
            }
        }

        #endregion
    }
}
