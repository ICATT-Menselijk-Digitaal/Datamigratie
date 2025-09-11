using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datamigratie.Common.Services.Shared.Models
{
    /// <summary>
    /// Represents a paginated response from the API.
    /// </summary>
    /// <typeparam name="T">The type of the objects in the results list.</typeparam>
    public class PagedResponse<T>
    {
        public int Count { get; set; }
        public bool NextPage { get; set; }
        public bool PreviousPage { get; set; }
        public List<T> Results { get; set; }
    }
}
