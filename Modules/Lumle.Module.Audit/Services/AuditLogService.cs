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

        public void Add(AuditLog entity)
        {
            try
            {
                _auditLogRepository.Add(entity);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.SaveError);
                throw;
            }
        }

        private void AddObjectAuditRecord(AuditLogModel auditLogModel)
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

                Add(audit);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.AuditLogError);
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
                throw;
            }
        }

        public List<AuditChange> GetAll(Expression<Func<AuditLog, bool>> predicate, string userTimezone)
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

                return result;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.DataFetchError);
                throw;
            }
        }

        private void AddStringAuditRecord(AuditLogModel auditLogModel)
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

                Add(audit);
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

        private void AddListAuditRecord(AuditLogModel auditLogModel)
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

                Add(audit);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.StringComparisonError);
                throw;
            }
        }

        public List<AuditChange> GetListRecords(Expression<Func<AuditLog, bool>> predicate, string userTimezone)
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

                return result;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.DataFetchError);
                throw;
            }
        }

        private void AddObjectListAuditRecord(AuditLogModel auditLogModel)
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

                Add(audit);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.AuditLogError);
                throw;
            }
        }

        public void Add(AuditLogModel auditLogModel)
        {
            try
            {
                if (auditLogModel == null) return;
                switch (auditLogModel.ComparisonType)
                {
                    case ComparisonType.ObjectCompare:
                        AddObjectAuditRecord(auditLogModel);
                        break;
                    case ComparisonType.StringCompare:
                        AddStringAuditRecord(auditLogModel);
                        break;
                    case ComparisonType.ListCompare:
                        AddListAuditRecord(auditLogModel);
                        break;
                    case ComparisonType.ObjectListCompare:
                        AddObjectListAuditRecord(auditLogModel);
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

    }
}