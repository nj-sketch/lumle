using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Lumle.Core.Models;
using Lumle.Data.Data.Abstracts;
using Lumle.Infrastructure.Constants.LumleLog;
using Lumle.Module.Localization.Models;
using NLog;

namespace Lumle.Module.Localization.Services
{
    public class ResourceService : IResourceService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IRepository<Culture> _cultureRespository;
        private readonly IRepository<Resource> _resourceRepository;

        public ResourceService(IRepository<Resource> resourceRepository, IRepository<Culture> cultureRespository)
        {
            _resourceRepository = resourceRepository;
            _cultureRespository = cultureRespository;
        }
        public void Add(Resource entity)
        {
            try
            {
                _resourceRepository.Add(entity);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.SaveError);
                throw new Exception(ex.Message);
            }
        }

        public void DeleteWhere(Expression<Func<Resource, bool>> predicate)
        {
            try
            {
                _resourceRepository.DeleteWhere(predicate);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.DeleteError);
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<Resource> GetAll()
        {
            try
            {
                return _resourceRepository.GetAll();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.DataFetchError);
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<Resource> GetAll(Expression<Func<Resource, bool>> predicate)
        {
            try
            {
                return _resourceRepository.GetAll(predicate);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.DataFetchError);
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<ResourceModel> GetAllResource(string culture)
        {
            try
            {
                var cultureEntity = _cultureRespository.GetSingle(x => x.Name == culture.Trim());
                if (cultureEntity == null) return null;

                var resource = (from e in _resourceRepository.GetAll()
                                group e by e.Key into f
                                select new ResourceModel
                                {
                                    CultureId = cultureEntity.Id,
                                    ResourceCategoryId = f.Select(x => x.ResourceCategoryId).FirstOrDefault(),
                                    Key = f.Key,
                                    Value = f.FirstOrDefault(x => x.CultureId == cultureEntity.Id).Value ?? ""
                                });

                return resource;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.DataFetchError);
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<ResourceModel> GetAllResource(int resourceCategoryId, string culture)
        {
            try
            {
                var cultureEntity = _cultureRespository.GetSingle(x => x.Name == culture.Trim());
                if (cultureEntity == null) return null;

                var resource = (from e in _resourceRepository.GetAll()
                                where e.ResourceCategoryId == resourceCategoryId
                                group e by e.Key into f
                                select new ResourceModel
                                {
                                    CultureId = cultureEntity.Id,
                                    ResourceCategoryId = f.Select(x => x.ResourceCategoryId).FirstOrDefault(),
                                    Key = f.Key,
                                    Value = f.FirstOrDefault(x => x.CultureId == cultureEntity.Id).Value ?? ""
                                });
                return resource;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.DataFetchError);
                throw new Exception(ex.Message);
            }
        }

        public Resource GetSingle(Expression<Func<Resource, bool>> predicate)
        {
            try
            {
                return _resourceRepository.GetSingle(predicate);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.DataFetchError);
                throw new Exception(ex.Message);
            }
        }

        public void Update(Resource entity)
        {
            try
            {
                _resourceRepository.Update(entity);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.UpdateError);
                throw new Exception(ex.Message);
            }
        }

        public bool IsCultureContainKey(string culture, int resourceCategoryId, string key)
        {
            try
            {
                var cultureContainKey = _resourceRepository.GetSingle(x => x.Culture.Name.ToLower() == culture.Trim().ToLower() && x.Key.ToLower() == key.Trim().ToLower() && x.ResourceCategoryId == resourceCategoryId);
                if (cultureContainKey != null)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.DataFetchError);
                throw new Exception(ex.Message);
            }
        }
    }
}
