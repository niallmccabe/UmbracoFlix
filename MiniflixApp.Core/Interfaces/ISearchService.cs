using Examine;
using MiniflixApp.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniflixApp.Core.Interfaces
{
    public interface ISearchService
    {
        IEnumerable<string> GetContentIdResults(SearchModel model);
        //IEnumerable<ISearchResult> GetResults(SearchModel model);
        ISearchResults GetResults(SearchModel model);

        ISearchResults GetAutoCompleteResults(SearchModel model);
    }
}
