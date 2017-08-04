using System;
using Lumle.Api.Infrastructures.Extensions;
using Lumle.Api.Infrastructures.Handlers.ApiResponse.Models;
using Microsoft.AspNetCore.Mvc;

namespace Lumle.Api.Infrastructures.Helpers
{
    public static class PaginationBuilder
    {
        public static Pagination Build(int totalObject, int pageSize, int page, string routeName, IUrlHelper urlHelper)
        {
            //calculate date for metadata
            var totalPages = (int)Math.Ceiling((double)totalObject / pageSize);

            //Prepare previous and next url
            var prevLink = page > 1 ? urlHelper.AbsoluteRouteUrl(routeName, new { Page = page - 1, PageSize = pageSize }) : "";
            var nextLink = page < totalPages ? urlHelper.AbsoluteRouteUrl(routeName, new { Page = page + 1, PageSize = pageSize }) : "";

            var paginationObj = new Pagination
            {
                CurrentPage = page,
                Size = pageSize,
                TotalObject = totalObject,
                Previous = prevLink,
                Next = nextLink
            };

            return paginationObj;
        }
    }
}
