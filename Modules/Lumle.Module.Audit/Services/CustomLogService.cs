using System;
using System.Linq.Expressions;
using Lumle.Data.Data.Abstracts;
using Lumle.Infrastructure.Constants.LumleLog;
using Lumle.Module.Audit.Entities;
using NLog;
using System.Linq;
using Lumle.Infrastructure.Helpers;
using Lumle.Module.Audit.ViewModels;
using System.Text.RegularExpressions;
using Lumle.Module.Audit.Helpers;
using AutoMapper;
using System.Collections.Generic;
using Lumle.Infrastructure.Utilities.Abstracts;
using Lumle.Data.Models;

namespace Lumle.Module.Audit.Services
{
    public class CustomLogService : ICustomLogService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IRepository<CustomLog> _customLogRepository;
        private readonly ITimeZoneHelper _timeZoneHelper;

        public CustomLogService(IRepository<CustomLog> customLogRepository, ITimeZoneHelper timeZoneHelper)
        {
            _customLogRepository = customLogRepository;
            _timeZoneHelper = timeZoneHelper;
        }

        public DataTableHelper.DTResult<CustomLogVM> GetDataTableResult(User loggedUser, CustomLogDTParameter parameters)
        {
            try
            {
                // Filter the records
                var customLogs = GetFilteredRecords(loggedUser.UserName, parameters.LogLevel, parameters.UserAgent, parameters.StartDate, parameters.EndDate);
                int totalAuditRecord;
                if (!string.IsNullOrEmpty(parameters.Search.Value.Trim())
                   && !string.IsNullOrWhiteSpace(parameters.Search.Value.Trim()))
                {
                    Expression<Func<CustomLog, bool>> search =
                        x => (x.Username ?? "").ToString().ToLower().Contains(parameters.Search.Value.ToLower()) ||
                             (x.Level ?? "").ToString().ToLower().Contains(parameters.Search.Value.ToLower()) ||
                             (x.UserAgent ?? "").ToString().ToLower().Contains(parameters.Search.Value.ToLower());

                    totalAuditRecord = customLogs.Count(search);
                    customLogs = customLogs.Where(search);
                    customLogs = SortByColumnWithOrder(parameters.Order[0].Column, parameters.Order[0].Dir.ToString(), customLogs);
                    customLogs = customLogs.Skip(parameters.Start).Take(parameters.Length);
                }
                else
                {
                    totalAuditRecord = customLogs.Count();
                    customLogs = SortByColumnWithOrder(parameters.Order[0].Column, parameters.Order[0].Dir.ToString(), customLogs);
                    customLogs = customLogs.Skip(parameters.Start).Take(parameters.Length);
                }
                var i = parameters.Start + 1;
                var customLogVms = Mapper.Map<List<CustomLogVM>>(customLogs);
                customLogVms = customLogVms.Select(x =>
                {
                    x.Sn = i++;
                    x.LoggedDate = _timeZoneHelper.ConvertToLocalTime(DateTime.Parse(x.LoggedDate), loggedUser.TimeZone).ToString("g");
                    return x;
                }).ToList();

                var dataTableResult = new DataTableHelper.DTResult<CustomLogVM>
                {
                    Draw = parameters.Draw,
                    Data = customLogVms,
                    RecordsFiltered = totalAuditRecord,
                    RecordsTotal = totalAuditRecord
                };

                return dataTableResult;
            }
            catch(Exception ex)
            {
                Logger.Error(ex, ErrorLog.DataFetchError);
                throw;
            }
        }

        #region Helpers

        private static string FormatLogMessage(string message)
        {
            const string pattern = @"CustomLog:";
            const string substitution = @"";

            var regex = new Regex(pattern);
            var result = regex.Replace(message, substitution, 1);

            return result;
        }

        /// <summary>
        /// Serverside dataTable Sorting
        /// </summary>
        /// <param name="order"></param>
        /// <param name="orderDirection"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private static IQueryable<CustomLog> SortByColumnWithOrder(int order, string orderDirection, IQueryable<CustomLog> data)
        {
            try
            {
                switch (order)
                {
                    case 2:
                        return orderDirection.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.Username) : data.OrderBy(p => p.Username);
                    case 3:
                        return orderDirection.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.Level) : data.OrderBy(p => p.Level);
                    default:
                        return data.OrderByDescending(p => p.LoggedDate);
                }
            }
            catch (Exception)
            {
                return data;
            }
        }

        private IQueryable<CustomLog> GetFilteredRecords(string usernameSearch, string logLevelSearch, string browserSearch, string startDate, string endDate)
        {
            var customLogEntities = _customLogRepository.GetAll()
                .Select(customLog => new CustomLog
                {
                    Id = customLog.Id,
                    Level = customLog.Level,
                    Username = customLog.Username,
                    UserAgent = customLog.UserAgent,
                    Message = FormatLogMessage(customLog.Message),
                    Url = customLog.Url,
                    RemoteAddress = customLog.RemoteAddress,
                    Exception = customLog.Exception,
                    LoggedDate = customLog.LoggedDate
                });

            if (!string.IsNullOrEmpty(usernameSearch))
            {
                customLogEntities = customLogEntities.Where(model => model.Username.Contains(usernameSearch));
            }

            if (!string.IsNullOrEmpty(browserSearch))
            {
                customLogEntities = customLogEntities.Where(model => model.UserAgent.Contains(browserSearch));
            }

            if (!string.IsNullOrEmpty(logLevelSearch))
            {
                customLogEntities = customLogEntities.Where(action => action.Level.Contains(logLevelSearch));
            }

            if (!string.IsNullOrEmpty(startDate))
            {
                customLogEntities = customLogEntities.Where(sD => DateTime.Parse(sD.LoggedDate) >= DateTime.Parse(startDate));
            }

            if (!string.IsNullOrEmpty(endDate))
            {
                customLogEntities = customLogEntities.Where(eD => DateTime.Parse(eD.LoggedDate) <= DateTime.Parse(endDate).AddDays(1));
            }

            return customLogEntities;
        }

        #endregion
    }
}
