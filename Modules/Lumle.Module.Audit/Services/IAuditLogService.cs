using Lumle.Data.Models;
using Lumle.Module.Audit.Entities;
using Lumle.Module.Audit.Helpers;
using Lumle.Module.Audit.Models;
using Lumle.Module.Audit.ViewModels;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static Lumle.Infrastructure.Helpers.DataTableHelper;

namespace Lumle.Module.Audit.Services
{
    public interface IAuditLogService
    {
        IQueryable<AuditLog> GetAll();
        IQueryable<AuditChange> GetAll(Expression<Func<AuditLog, bool>> predicate, string userTimezone);

        /// <summary>
        /// To return view where two list has been compared
        /// </summary>
        /// <param name="predicate">query that returns the list where two list has been compared</param>
        /// <returns></returns>
        IQueryable<AuditChange> GetListRecords(Expression<Func<AuditLog, bool>> predicate, string userTimezone);
        Task Create(AuditLogModel auditLogModel);

        DTResult<AuditLogVM> GetDataTableResult(User loggedUser, AuditLogDTParameter parameters);
    }
}
