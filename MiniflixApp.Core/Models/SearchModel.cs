using Examine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniflixApp.Core.Models
{
    public class SearchModel
    {
        // Query Parameters
        public string SearchTerm { get; set; }
        public IEnumerable<string> SearchTerms { get; set; }
        public IEnumerable<string> IgnoreDocumentTypes { get; set; }
        public IEnumerable<string> OnlyDocumentTypes { get; set; }
        public int CurrentPage { get; set; }

        // Options
        public int RootContentNodeId { get; set; }
        public int RootMediaNodeId { get; set; }
        public string IndexType { get; set; }
        public IList<string> SearchFields { get; set; }
        public Dictionary<string, IEnumerable<string>> SearchFilters { get; set; }
        public bool FuzzySearch { get; set; }
        
        public string HideFromSearchField { get; set; }
        public string SearchFormLocation { get; set; }
        public string SearcherAlias { get; set; }

        // Results
        public int TotalResults { get; set; }
        
        public int TotalPages { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public ISearchResults AllResults { get; set; }
    }

}
