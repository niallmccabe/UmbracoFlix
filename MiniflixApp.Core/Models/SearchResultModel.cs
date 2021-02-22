using MiniflixApp.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniflixApp.Core.Models
{
    public class SearchResultModel
    {
        public string Name { get; set; }
        public SearchResultType Type { get; set; }
        public string LinkUrl { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
    }
}
