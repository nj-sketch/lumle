using System;
using System.Linq.Expressions;
using Lumle.Module.PublicUser.Entities;
using Lumle.Data.Data.Abstracts;
using NLog;
using Lumle.Infrastructure.Constants.LumleLog;
using System.Threading.Tasks;
using System.Linq;
using Lumle.Data.Models;
using Lumle.Infrastructure.Helpers;
using Lumle.Module.PublicUser.Helpers;
using Lumle.Module.PublicUser.ViewModels.PublicUserViewModels;
using System.Collections.Generic;
using Lumle.Infrastructure.Utilities.Abstracts;
using static Lumle.Infrastructure.Helpers.DataTableHelper;
using Lumle.Module.Audit.Models;
using Lumle.Module.Audit.Enums;
using Lumle.Module.Audit.Services;

namespace Lumle.Module.PublicUser.Services
{
    public class PublicUserService : IPublicUserService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IAuditLogService _auditLogService;
        private readonly IRepository<CustomUser> _customUserRepository;
        private readonly ITimeZoneHelper _timeZoneHelper;

        public PublicUserService(
            IRepository<CustomUser> customUserRepository,
            ITimeZoneHelper timeZoneHelper,
            IAuditLogService auditLogService
        )
        {
            _customUserRepository = customUserRepository;
            _timeZoneHelper = timeZoneHelper;
            _auditLogService = auditLogService;
        }

        public async Task Update(PublicUserEditVM model, User loggedUser)
        {
            try
            {
                var publicUser = await _customUserRepository.GetSingleAsync(x => x.Id == Convert.ToInt32(model.Id));

                var oldRecord = new CustomUser
                {
                    Id = publicUser.Id,
                    IsBlocked = publicUser.IsBlocked,
                    IsStaff = publicUser.IsStaff,
                    LastUpdated = publicUser.LastUpdated,
                };

                publicUser.IsBlocked = Convert.ToBoolean(model.IsBlocked);
                publicUser.IsStaff = Convert.ToBoolean(model.IsStaff);
                publicUser.LastUpdated = DateTime.UtcNow;

                await _customUserRepository.UpdateAsync(publicUser, publicUser.Id);
                await _customUserRepository.SaveChangesAsync();

                #region Public User Audit log
                var auditLogModel = new AuditLogModel
                {
                    AuditActionType = AuditActionType.Update,
                    KeyField = publicUser.Id.ToString(),
                    OldObject = oldRecord,
                    NewObject = publicUser,
                    LoggedUserEmail = loggedUser.Email,
                    ComparisonType = ComparisonType.ObjectCompare
                };
                await _auditLogService.Create(auditLogModel);
                #endregion
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.UpdateError);
                throw;
            }
        }

        public DataTableHelper.DTResult<PublicUserIndexVM> GetDataTableResult(User loggedUser, PublicUserDTParamaters parameters)
        {
            try
            {
                var publicUsers = _customUserRepository.GetAll().Select(usr => new PublicUserIndexVM
                {
                    Id = usr.Id,
                    UserName = usr.UserName,
                    ProfileUrl = usr.ProfileUrl,
                    Email = usr.Email,
                    Gender = usr.Gender,
                    IsStaff = usr.IsStaff,
                    IsEmailVerified = usr.IsEmailVerified,
                    IsBlocked = usr.IsBlocked,
                    Provider = usr.Provider,
                    CreatedDate = usr.CreatedDate
                }).AsQueryable();

                List<PublicUserIndexVM> filteredUsers;
                int totalUser;
                if (!string.IsNullOrEmpty(parameters.Search.Value.Trim())
                   && !string.IsNullOrWhiteSpace(parameters.Search.Value.Trim()))
                {
                    Expression<Func<PublicUserIndexVM, bool>> search =
                        x => (x.UserName ?? "").ToString().ToLower().Contains(parameters.Search.Value.ToLower()) ||
                             (x.Email ?? "").ToString().ToLower().Contains(parameters.Search.Value.ToLower()) ||
                             (x.Gender.ToString()).Contains(parameters.Search.Value.ToLower()) ||
                             (x.Provider ?? "").ToString().ToLower().Contains(parameters.Search.Value.ToLower());
                    totalUser = publicUsers.Count(search);
                    publicUsers = publicUsers.Where(search);
                    publicUsers = SortByColumnWithOrder(parameters.Order[0].Column, parameters.Order[0].Dir.ToString(), publicUsers);
                    filteredUsers = publicUsers.Skip(parameters.Start).Take(parameters.Length).ToList();
                }
                else
                {
                    totalUser = publicUsers.Count();
                    publicUsers = SortByColumnWithOrder(parameters.Order[0].Column, parameters.Order[0].Dir.ToString(), publicUsers);
                    filteredUsers = publicUsers.Skip(parameters.Start).Take(parameters.Length).ToList();
                }
                var counter = parameters.Start + 1;
                filteredUsers = filteredUsers.Select(x => {
                    x.SN = counter++;
                    x.FormatedCreatedDate = _timeZoneHelper.ConvertToLocalTime(x.CreatedDate, loggedUser.TimeZone).ToString("g");
                    return x;
                }).ToList();

                var datatableResult = new DTResult<PublicUserIndexVM>
                {
                    Draw = parameters.Draw,
                    Data = filteredUsers,
                    RecordsFiltered = totalUser,
                    RecordsTotal = totalUser
                };

                return datatableResult;
            }
            catch(Exception ex)
            {
                Logger.Error(ex, ErrorLog.DataFetchError);
                throw;
            }
        }

        private static IQueryable<PublicUserIndexVM> SortByColumnWithOrder(int order, string orderDirection, IQueryable<PublicUserIndexVM> data)
        {
            try
            {
                switch (order)
                {
                    case 2:
                        return orderDirection.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.UserName) : data.OrderBy(p => p.UserName);
                    case 3:
                        return orderDirection.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.Email) : data.OrderBy(p => p.Email);
                    case 4:
                        return orderDirection.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.Gender) : data.OrderBy(p => p.Gender);
                    case 5:
                        return orderDirection.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.IsStaff) : data.OrderBy(p => p.IsStaff);
                    case 6:
                        return orderDirection.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.IsEmailVerified) : data.OrderBy(p => p.IsEmailVerified);
                    case 7:
                        return orderDirection.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.IsBlocked) : data.OrderBy(p => p.IsBlocked);
                    case 8:
                        return orderDirection.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.Provider) : data.OrderBy(p => p.Provider);
                    case 9:
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
    }
}
