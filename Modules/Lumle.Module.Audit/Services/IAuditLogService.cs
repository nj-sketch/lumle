using Lumle.Module.Audit.Entities;
using Lumle.Module.Audit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Lumle.Module.Audit.Services
{
    public interface IAuditLogService
    {
        IQueryable<AuditLog> GetAll();  
        void Add(AuditLog entity);
        List<AuditChange> GetAll(Expression<Func<AuditLog, bool>> predicate, string userTimezone);

        /// <summary>
        /// To return view where two list has been compared
        /// </summary>
        /// <param name="predicate">query that returns the list where two list has been compared</param>
        /// <returns></returns>
        List<AuditChange> GetListRecords(Expression<Func<AuditLog, bool>> predicate, string userTimezone);
        void Add(AuditLogModel auditLogModel);
    }
}
