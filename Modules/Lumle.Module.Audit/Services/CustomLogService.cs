using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Lumle.Data.Data.Abstracts;
using Lumle.Infrastructure.Constants.LumleLog;
using Lumle.Module.Audit.Entities;
using NLog;
using System.Linq;

namespace Lumle.Module.Audit.Services
{
    public class CustomLogService : ICustomLogService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IRepository<CustomLog> _customLogRepository;

        public CustomLogService(IRepository<CustomLog> customLogRepository)
        {
            _customLogRepository = customLogRepository;
        }

        public IEnumerable<CustomLog> GetAll(Expression<Func<CustomLog, bool>> predicate)
        {
            try
            {
                var customLogs = _customLogRepository.GetAll(predicate);
                return customLogs;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.DataFetchError);
                throw new Exception(ex.Message);
            }
        }

        public IQueryable<CustomLog> GetAll()
        {
            try
            {
                var customLogs = _customLogRepository.GetAll();
                return customLogs;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.DataFetchError);
                throw new Exception(ex.Message);

            }
        }
    }
}
