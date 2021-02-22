using MiniflixApp.Core.Interfaces;
using MiniflixApp.Core.Models;
using MiniflixApp.Core.Models.ViewModels;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco.Web;
using Umbraco.Web.Mvc;
using Constants = Umbraco.Core.Constants;

namespace MiniflixApp.Web.Controllers.Surface
{
    public class SearchController : SurfaceController
    {
        private readonly ISearchService _searchService;
        private readonly ITagQuery _tagQuery;
        public SearchController(ISearchService searchService, ITagQuery tagQuery)
        {
            _searchService = searchService;
            _tagQuery = tagQuery;
        }
        public ActionResult Search()
        {            
            var allTags = _tagQuery.GetAllContentTags().GroupBy(x => x.Group);
            var filterOptions = new Dictionary<string, IEnumerable<string>>();
            foreach(var tag in allTags)
            {
                filterOptions.Add(tag.Key, tag.Select(x => x.Text));
            }
            var model = new SearchViewModel()
            {
                FilterOptions = filterOptions
            };

            return PartialView("Search", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Search(SearchViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }
            NameValueCollection queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString.Add("searchTerm", model.SearchTerm);
            if (model.SelectedFilters != null)
            {
                var selectedFiltersValue = JsonConvert.SerializeObject(model.SelectedFilters);
                queryString.Add("selectedFilters", selectedFiltersValue);
            }
            
            var searchPage = CurrentPage.Root().DescendantsOfType("search").FirstOrDefault();
            return RedirectToUmbracoPage(searchPage, queryString);
        }

        public ActionResult GetSearchResults()
        {
            var searchTerm = Request.QueryString["searchTerm"];
            var searchFilters = Request.QueryString["searchFilters"];
            var filters = !string.IsNullOrEmpty(searchFilters) ? JsonConvert.DeserializeObject<Dictionary<string, IEnumerable<string>>>(searchFilters) : null;

            var results = _searchService.GetResults(new SearchModel
            {
                SearchTerm = searchTerm,
                SearchFilters = filters,
                IndexType = Constants.UmbracoIndexes.ExternalIndexName
            });
            var searchResults = results?.Select(x => {
                var node = Umbraco.Content(x.Id);
                return new SearchResultModel
                {
                    Description = x.Values["description"],
                    Name = x.Values["nodeName"],
                    LinkUrl = node.Url()
                };
            });
            return PartialView("SearchResults", new SearchResultsViewModel { SearchResults = searchResults });
        }
    }
}