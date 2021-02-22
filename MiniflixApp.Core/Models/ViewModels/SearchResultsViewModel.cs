using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniflixApp.Core.Models.ViewModels
{
    public class SearchResultsViewModel
    {
        public IEnumerable<SearchResultModel> SearchResults { get; set; }
    }
}
