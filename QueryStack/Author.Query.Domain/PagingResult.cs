using System;
using System.Collections.Generic;
using System.Text;

namespace Author.Query.Domain
{
    public class PagingResult
    {
        /// <summary>Gets or sets total records count in the collection</summary>
        /// <value>It is of type Int</value>
        //[JsonProperty("totalCount")]
        public int TotalCount { get; set; }

        /// <summary>Gets or sets page size of the result</summary>
        /// <value>It is of type Int</value>
        //[JsonProperty("pageSize")]
        public int PageSize { get; set; }

        /// <summary>Gets or sets the current page number in the records collection</summary>
        /// <value>It is of type Int</value>
        //[JsonProperty("pageNumber")]
        public int PageNumber { get; set; }

        /// <summary>Gets or sets total pages in current collection (total records/page Size)</summary>
        /// <value>It is of type Int</value>
        //[JsonProperty("totalPages")]
        public int TotalPages { get; set; }

    }
}
