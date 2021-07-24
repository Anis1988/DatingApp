using Microsoft.AspNetCore.Http;
using API.Helpers;
using System.Text.Json;

namespace API.Extensions
{
    public static class HttpExtentions
    {
        public static void AddPaginationHeader(this HttpResponse response ,
                        int currentPage,int itemsPerPage,int totalItems,int totalPages)
        {
            var PaginationHeader = new PaginationHeader(currentPage,itemsPerPage,totalItems,totalPages);

            // this is just to make it look nice
            var options = new JsonSerializerOptions
            {
               PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            response.Headers.Add("Pagination", JsonSerializer.Serialize(PaginationHeader,options));
            response.Headers.Add("Access-Control-Expose-Headers","Pagination");
        }
    }
}
