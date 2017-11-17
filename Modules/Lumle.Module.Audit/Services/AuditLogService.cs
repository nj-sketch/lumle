using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using KellermanSoftware.CompareNetObjects;
using Lumle.Data.Data.Abstracts;
using Lumle.Infrastructure.Constants.LumleLog;
using Lumle.Module.Audit.Entities;
using Lumle.Module.Audit.Enums;
using Lumle.Module.Audit.Models;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Newtonsoft.Json;
using NLog;
using Lumle.Infrastructure.Utilities.Abstracts;
using System.Threading.Tasks;
using Lumle.Data.Models;
using Lumle.Infrastructure.Helpers;
using Lumle.Module.Audit.Helpers;
using Lumle.Module.Audit.ViewModels;
using AutoMapper;

namespace Lumle.Module.Audit.Services
{
    public class AuditLogService : IAuditLogService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IRepository<AuditLog> _auditLogRepository;
        private readonly ITimeZoneHelper _timeZoneHelper;

        #region Class Variables
        private CompareLogic _compare;
        private readonly string _ipAddress;
        private readonly string _userAgent;
        #endregion

        public AuditLogService(
            IRepository<AuditLog> auditLogRepository,
            IActionContextAccessor accessor,
            ITimeZoneHelper timeZoneHelper
            )
        {
            _auditLogRepository = auditLogRepository;
            _ipAddress = accessor.ActionContext.HttpContext.Request.HttpContext.Connection.RemoteIpAddress.ToString();
            _userAgent = accessor.ActionContext.HttpContext.Request.Headers["User-Agent"].ToString();
            _timeZoneHelper = timeZoneHelper;
            Initialize();
        }

        private async Task AddObjectAuditRecord(AuditLogModel auditLogModel)
        {
            try
            {
                var modelName = auditLogModel.OldObject.GetType().Name;               

                var compResult = _compare.Compare(auditLogModel.OldObject, auditLogModel.NewObject);

                var auditInfoList = new List<AuditInfo>();

                if (compResult.Differences.Count == 0) return;
                foreach (var change in compResult.Differences)
                {
                    var auditInfo = new AuditInfo();
                    if (change.PropertyName.Substring(0, 1) == ".")
                        auditInfo.FieldName = change.PropertyName.Substring(1, change.PropertyName.Length - 1);
                    auditInfo.ValueBefore = change.Object1Value;
                    auditInfo.ValueAfter = change.Object2Value;
                    auditInfoList.Add(auditInfo);
                }

                string auditSummary;
                switch (auditLogModel.AuditActionType)
                {
                    case AuditActionType.Create:
                        auditSummary = "A new record has been added in " + modelName + " module by " + auditLogModel.LoggedUserEmail;
                        break;
                    case AuditActionType.Update:
                        auditSummary = "An exisiting record has been updated in " + modelName + " module by " + auditLogModel.LoggedUserEmail;
                        break;
                    case AuditActionType.Delete:
                        auditSummary = "A record has been deleted in " + modelName + " module by " + auditLogModel.LoggedUserEmail;
                        break;
                    default:
                        Logger.Fatal(ErrorLog.AuditActionError);
                        throw new NotImplementedException();
                }
                var audit = new AuditLog
                {
                    AuditType = auditLogModel.AuditActionType.ToString(),
                    TableName = modelName,
                    CreatedDate = DateTime.UtcNow,
                    LastUpdated = DateTime.UtcNow,
                    KeyField = auditLogModel.KeyField,
                    IpAddress = _ipAddress,
                    UserAgent = _userAgent,
                    UserId = auditLogModel.LoggedUserEmail,
                    OldValue = JsonConvert.SerializeObject(auditLogModel.OldObject),
                    NewValue = JsonConvert.SerializeObject(auditLogModel.NewObject),
                    Changes = JsonConvert.SerializeObject(auditInfoList),
                    AuditSummary = auditSummary
                };

                await _auditLogRepository.AddAsync(audit);
                await _auditLogRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.AuditLogError);
                throw;
            }
        }

        public IQueryable<AuditChange> GetAll(Expression<Func<AuditLog, bool>> predicate, string userTimezone)
        {
            try
            {
                var result = new List<AuditChange>();
                var auditRecords = _auditLogRepository.GetAll(predicate);
                foreach (var record in auditRecords)
                {
                    var change = new AuditChange
                    {
                        AuditId = record.Id,
                        CreatedDate = _timeZoneHelper.ConvertToLocalTime(record.CreatedDate, userTimezone).ToString("g"),
                        ActionPerformedBy = record.UserId,
                        AuditActionTypeName = record.AuditType
                    };
                    if (IsJson(record.Changes))
                    {
                        var delta = JsonConvert.DeserializeObject<List<AuditInfo>>(record.Changes);
                        change.AuditInfos.AddRange(delta);
                    }
                    else
                    {
                        var auditInfo = new AuditInfo
                        {
                            FieldName = record.TableName,
                            ValueBefore = record.OldValue,
                            ValueAfter = record.NewValue
                        };
                        change.AuditInfos.Add(auditInfo);
                    }

                    result.Add(change);
                }

                return result.AsQueryable();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.DataFetchError);
                throw;
            }
        }

        private async Task AddStringAuditRecord(AuditLogModel auditLogModel)
        {
            try
            {
                string auditSummary;
                string auditChanges;
                var oldAuditRecord = auditLogModel.OldString;
                var newAuditRecord = auditLogModel.NewString;
                var modelName = auditLogModel.ModuleList.ToString();
                switch (auditLogModel.AuditActionType)
                {
                    case AuditActionType.Create:
                        auditChanges = "No Changes";
                        oldAuditRecord = "N/A";
                        auditSummary = "A new record has been added in " + modelName + " module by " + auditLogModel.LoggedUserEmail;
                        break;
                    case AuditActionType.Update:
                        if (oldAuditRecord.Equals(newAuditRecord, StringComparison.OrdinalIgnoreCase)) return;

                        auditChanges = oldAuditRecord + " Changed to " + newAuditRecord;
                        auditSummary = "An exisiting record has been updated in " + modelName + " module by " + auditLogModel.LoggedUserEmail;
                        break;
                    case AuditActionType.Delete:
                        auditChanges = oldAuditRecord + " Changed to null";
                        newAuditRecord = "N/A";

                        auditSummary = "A record has been deleted in " + modelName + " module by " + auditLogModel.LoggedUserEmail;
                        break;
                    default:
                        Logger.Fatal(ErrorLog.AuditActionError);
                        throw new NotImplementedException();
                }
                var audit = new AuditLog
                {
                    AuditType = auditLogModel.AuditActionType.ToString(),
                    KeyField = auditLogModel.KeyField,
                    TableName = modelName,
                    IpAddress = _ipAddress,
                    UserAgent = _userAgent,
                    UserId = auditLogModel.LoggedUserEmail,
                    CreatedDate = DateTime.UtcNow,
                    LastUpdated = DateTime.UtcNow,
                    AuditSummary = auditSummary,
                    Changes = auditChanges,
                    OldValue = oldAuditRecord,
                    NewValue = newAuditRecord
                };

                await _auditLogRepository.AddAsync(audit);
                await _auditLogRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.StringComparisonError);
                throw;
            }
        }

        private void Initialize()
        {
            _compare = new CompareLogic();
            _compare.Config.MembersToIgnore.Add("CreatedDate");
            _compare.Config.MembersToIgnore.Add("LastUpdated");
            _compare.Config.MembersToIgnore.Add("PasswordHash");
            _compare.Config.MembersToIgnore.Add("ConcurrencyStamp");
            _compare.Config.MembersToIgnore.Add("SecurityStamp");
            _compare.Config.MaxDifferences = 99;

        }

        private static bool IsJson(string input)
        {
            input = input.Trim();
            return input.StartsWith("{") && input.EndsWith("}")
                   || input.StartsWith("[") && input.EndsWith("]");
        }

        private async Task AddListAuditRecord(AuditLogModel auditLogModel)
        {
            try
            {
                if (auditLogModel.OldStringList == null || auditLogModel.NewStringList == null) return;

                // Compare two list and get the differences
                string auditSummary;
                List<string> auditChanges;
                var modelName = auditLogModel.ModuleList.ToString();
                switch (auditLogModel.AuditActionType)
                {
                    case AuditActionType.Create:
                        auditChanges = auditLogModel.NewStringList;
                        auditSummary = "A new record has been added in " + modelName + " module by " + auditLogModel.LoggedUserEmail;
                        break;
                    case AuditActionType.Update:
                        var difference =
                            auditLogModel.NewStringList.Where(b => !auditLogModel.OldStringList.Any(b.SequenceEqual)).ToList();

                        if (difference.Count == 0) return;
                        auditChanges = difference;
                        auditSummary = "An exisiting record has been updated in " + modelName + " module by " + auditLogModel.LoggedUserEmail;
                        break;
                    case AuditActionType.Delete:
                        throw new NotImplementedException();
                    default:
                        Logger.Fatal(ErrorLog.AuditActionError);
                        throw new NotImplementedException();
                }

                var audit = new AuditLog
                {
                    AuditType = auditLogModel.AuditActionType.ToString(),
                    KeyField = auditLogModel.KeyField,
                    TableName = modelName,
                    IpAddress = _ipAddress,
                    UserAgent = _userAgent,
                    UserId = auditLogModel.LoggedUserEmail,
                    CreatedDate = DateTime.UtcNow,
                    LastUpdated = DateTime.UtcNow,
                    AuditSummary = auditSummary,
                    Changes = JsonConvert.SerializeObject(auditChanges),
                    OldValue = JsonConvert.SerializeObject(auditLogModel.OldStringList),
                    NewValue = JsonConvert.SerializeObject(auditLogModel.NewStringList),
                    ModuleInfo = auditLogModel.AdditionalInfo
                };

                await _auditLogRepository.AddAsync(audit);
                await _auditLogRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.StringComparisonError);
                throw;
            }
        }

        public IQueryable<AuditChange> GetListRecords(Expression<Func<AuditLog, bool>> predicate, string userTimezone)
        {
            try
            {
                var result = new List<AuditChange>();
                var auditRecords = _auditLogRepository.GetAll(predicate);

                foreach (var record in auditRecords)
                {
                    var auditChange = new AuditChange
                    {
                        AuditId = record.Id,
                        CreatedDate = _timeZoneHelper.ConvertToLocalTime(record.CreatedDate, userTimezone).ToString("g"),
                        ActionPerformedBy = record.UserId,
                        AuditActionTypeName = record.AuditType
                    };
                    var auditInfo = new AuditInfo
                    {
                        FieldName = record.ModuleInfo,
                        ValueBefore = record.OldValue,
                        ValueAfter = record.NewValue
                    };

                    auditChange.AuditInfo = auditInfo;

                    result.Add(auditChange);
                }

                return result.AsQueryable();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.DataFetchError);
                throw;
            }
        }

        private async Task AddObjectListAuditRecord(AuditLogModel auditLogModel)
        {
            try
            {
                var modelName = auditLogModel.OldObject.GetType().Name;

                var compResult = _compare.Compare(auditLogModel.OldObject, auditLogModel.NewObject);
               
                var auditInfoList = new List<AuditInfo>();

                var result = auditLogModel.NewStringList.Except(auditLogModel.OldStringList).ToList();
                if (result.Count == 0 && compResult.Differences.Count == 0) return;
                if (result.Count != 0)
                {
                    var listAuditInfo = new AuditInfo
                    {
                        FieldName = auditLogModel.AdditionalInfo,
                        ValueBefore = JsonConvert.SerializeObject(auditLogModel.OldStringList),
                        ValueAfter = JsonConvert.SerializeObject(auditLogModel.NewStringList)
                    };
                    auditInfoList.Add(listAuditInfo);
                }
             
                foreach (var change in compResult.Differences)
                {
                    var auditInfo = new AuditInfo();
                    if (change.PropertyName.Substring(0, 1) == ".")
                        auditInfo.FieldName = change.PropertyName.Substring(1, change.PropertyName.Length - 1);
                    auditInfo.ValueBefore = change.Object1Value;
                    auditInfo.ValueAfter = change.Object2Value;
                    auditInfoList.Add(auditInfo);
                }

                var audit = new AuditLog
                {
                    AuditType = auditLogModel.AuditActionType.ToString(),
                    TableName = modelName,
                    CreatedDate = DateTime.UtcNow,
                    LastUpdated = DateTime.UtcNow,
                    KeyField = auditLogModel.KeyField,
                    IpAddress = _ipAddress,
                    UserAgent = _userAgent,
                    UserId = auditLogModel.LoggedUserEmail,
                    OldValue = JsonConvert.SerializeObject(auditLogModel.OldObject),
                    NewValue = JsonConvert.SerializeObject(auditLogModel.NewObject),
                    Changes = JsonConvert.SerializeObject(auditInfoList),
                    AuditSummary = "An exisiting record has been updated in " + modelName + " module by " + auditLogModel.LoggedUserEmail
                };

                await _auditLogRepository.AddAsync(audit);
                await _auditLogRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.AuditLogError);
                throw;
            }
        }

        public async Task Create(AuditLogModel auditLogModel)
        {
            try
            {
                if (auditLogModel == null) return;
                switch (auditLogModel.ComparisonType)
                {
                    case ComparisonType.ObjectCompare:
                        await AddObjectAuditRecord(auditLogModel);
                        break;
                    case ComparisonType.StringCompare:
                        await AddStringAuditRecord(auditLogModel);
                        break;
                    case ComparisonType.ListCompare:
                        await AddListAuditRecord(auditLogModel);
                        break;
                    case ComparisonType.ObjectListCompare:
                        await AddObjectListAuditRecord(auditLogModel);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IQueryable<AuditLog> GetAll()
        {
            try
            {
                return _auditLogRepository.GetAll();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.DataFetchError);
                throw new Exception(ex.Message);
            }
        }

        public DataTableHelper.DTResult<AuditLogVM> GetDataTableResult(User loggedUser, AuditLogDTParameter parameters)
        {
            try
            {
                // Filter the records
                var auditLogs = GetFilteredRecords(parameters.ModuleName, parameters.ActionName, parameters.StartDate, parameters.EndDate, loggedUser.UserName, parameters.FieldName);
                int totalAuditRecord;
                if (!string.IsNullOrEmpty(parameters.Search.Value.Trim())
                   && !string.IsNullOrWhiteSpace(parameters.Search.Value.Trim()))
                {
                    Expression<Func<AuditLog, bool>> search =
                        x => (x.AuditType ?? "").ToString().ToLower().Contains(parameters.Search.Value.ToLower()) ||
                             (x.TableName ?? "").ToString().ToLower().Contains(parameters.Search.Value.ToLower()) ||
                             (x.UserId ?? "").ToString().ToLower().Contains(parameters.Search.Value.ToLower());

                    totalAuditRecord = auditLogs.Count(search);
                    auditLogs = auditLogs.Where(search);
                    auditLogs = SortByColumnWithOrder(parameters.Order[0].Column, parameters.Order[0].Dir.ToString(), auditLogs);
                    auditLogs = auditLogs.Skip(parameters.Start).Take(parameters.Length);
                }
                else
                {
                    totalAuditRecord = auditLogs.Count();
                    auditLogs = SortByColumnWithOrder(parameters.Order[0].Column, parameters.Order[0].Dir.ToString(), auditLogs);
                    auditLogs = auditLogs.Skip(parameters.Start).Take(parameters.Length);
                }
                var i = parameters.Start + 1;
                var auditLogVms = Mapper.Map<List<AuditLogVM>>(auditLogs);
                auditLogVms = auditLogVms.Select(x =>
                {
                    x.Sn = i++;
                    x.ConvertedCreatedDate = _timeZoneHelper.ConvertToLocalTime(x.CreatedDate, loggedUser.TimeZone).ToString("g");
                    return x;
                }).ToList();

                var datatableResult = new DataTableHelper.DTResult<AuditLogVM>
                {
                    Draw = parameters.Draw,
                    Data = auditLogVms,
                    RecordsFiltered = totalAuditRecord,
                    RecordsTotal = totalAuditRecord
                };

                return datatableResult;
            }
            catch(Exception ex)
            {
                Logger.Error(ex, ErrorLog.DataFetchError);
                throw new Exception(ex.Message);
            }
        }

        #region Helpers
        /// <summary>
        /// Serverside dataTable Sorting
        /// </summary>
        /// <param name="order"></param>
        /// <param name="orderDirection"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private static IQueryable<AuditLog> SortByColumnWithOrder(int order, string orderDirection, IQueryable<AuditLog> data)
        {
            try
            {
                switch (order)
                {
                    case 2:
                        return orderDirection.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.TableName) : data.OrderBy(p => p.TableName);
                    case 3:
                        return orderDirection.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.UserId) : data.OrderBy(p => p.UserId);
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

        private IQueryable<AuditLog> GetFilteredRecords(string modelSearch, string actionSearch, string startDate, string endDate, string userSearch, string fieldSearch)
        {

            var auditEntites = _auditLogRepository.GetAll()
                .Select(auditLog => new AuditLog
                {
                    Id = auditLog.Id,
                    AuditType = auditLog.AuditType,
                    AuditSummary = auditLog.AuditSummary,
                    CreatedDate = auditLog.CreatedDate,
                    TableName = auditLog.TableName,
                    ModuleInfo = auditLog.ModuleInfo,
                    Changes = auditLog.Changes,
                    UserId = auditLog.UserId,
                    KeyField = auditLog.KeyField
                });
            if (!string.IsNullOrEmpty(modelSearch))
            {
                auditEntites = auditEntites.Where(model => model.TableName.Contains(modelSearch.ToLower()));
            }

            if (!string.IsNullOrEmpty(actionSearch))
            {
                auditEntites = auditEntites.Where(action => action.AuditType.Contains(actionSearch.ToLower()));
            }

            if (!string.IsNullOrEmpty(userSearch))
            {
                auditEntites = auditEntites.Where(user => user.UserId.Contains(userSearch.ToLower()));
            }

            if (!string.IsNullOrEmpty(startDate))
            {
                auditEntites = auditEntites.Where(sD => sD.CreatedDate >= DateTime.Parse(startDate));
            }

            if (!string.IsNullOrEmpty(endDate))
            {
                auditEntites = auditEntites.Where(eD => eD.CreatedDate <= DateTime.Parse(endDate).AddDays(1));
            }

            if (!string.IsNullOrEmpty(fieldSearch))
            {
                auditEntites = auditEntites.Where(fS => fS.Changes.Contains("\"FieldName\":") && fS.Changes.ToLower().Contains(fieldSearch.ToLower()));
            }

            return auditEntites;
        }
        #endregion
    }
}