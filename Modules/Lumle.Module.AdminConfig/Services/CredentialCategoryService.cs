using System;
using Lumle.Module.AdminConfig.Entities;
using Lumle.Data.Data.Abstracts;
using Lumle.Module.AdminConfig.Models;
using System.Linq;
using Lumle.Infrastructure.Constants.LumleLog;
using NLog;
using System.Text.RegularExpressions;

namespace Lumle.Module.AdminConfig.Services
{
    public class CredentialCategoryService : ICredentialCategoryService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IRepository<CredentialCategory> _credentialCategoryRepository;

        public CredentialCategoryService(IRepository<CredentialCategory> credentialCategoryRepository)
        {
            _credentialCategoryRepository = credentialCategoryRepository;
        }

        public IQueryable<CredentialCategoryModel> GetAllCredentialCategory()
        {
            try
            {
                var i = 0;
                var data = (from e in _credentialCategoryRepository.GetAll()
                            select new CredentialCategoryModel
                            {
                                Id = e.Id,
                                Name=e.Name,
                                NameIdentifier = Regex.Replace(e.Name, @"\s+", "")
                            }).ToList();

                return data.Select(x => { x.Sn = ++i; return x; }).AsQueryable();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.DataFetchError);
                throw;
            }
        }
    }
}
