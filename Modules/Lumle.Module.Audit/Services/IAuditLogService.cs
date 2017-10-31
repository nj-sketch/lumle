using Lumle.Module.Audit.Entities;
using Lumle.Module.Audit.Models;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Lumle.Module.Audit.Services
{
    public interface IAuditLogService
    {
        IQueryable<AuditLog> GetAll();  
        Task Add(AuditLog entity);
        IQueryable<AuditChange> GetAll(Expression<Func<AuditLog, bool>> predicate, string userTimezone);

        /// <summary>
        /// To return view where two list has been compared
        /// </summary>
        /// <param name="predicate">query that returns the list where two list has been compared</param>
        /// <returns></returns>
        IQueryable<AuditChange> GetListRecords(Expression<Func<AuditLog, bool>> predicate, string userTimezone);
        Task Add(AuditLogModel auditLogModel);
    }
}
