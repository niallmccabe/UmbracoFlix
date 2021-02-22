using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniflixApp.Core.Models.ViewModels
{
    public class SearchViewModel
    {
        public string SearchTerm { get; set; }
        public Dictionary<string, IEnumerable<string>> FilterOptions { get; set; }
        public Dictionary<string, IEnumerable<string>> SelectedFilters { get; set; }
    }
}
