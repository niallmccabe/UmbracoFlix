using Examine;
using Examine.Search;
using MiniflixApp.Core.Interfaces;
using MiniflixApp.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Umbraco.Examine;
using Umbraco.Web;

namespace MiniflixApp.Core.Services
{
    public class ExamineSearchService : ISearchService
    {
        private UmbracoHelper _umbracoHelper;
        public ExamineSearchService(UmbracoHelper umbracoHelper)
        {
            _umbracoHelper = umbracoHelper;
        }

        public ISearchResults GetAutoCompleteResults(SearchModel model)
        {
            if (!string.IsNullOrEmpty(model.SearchTerm))
            {
                var searchResults = GetResults(model);       
                return searchResults;
            }

            return null;
        }

        public IEnumerable<string> GetContentIdResults(SearchModel model)
        {
            var results = GetResults(model);
            if (results != null && results.Any())
            {
                return results.Select(x => x.Values["id"]);
            }
            return null;
        }
        public ISearchResults GetResults(SearchModel model)
        {
            if(model != null && ExamineManager.Instance.TryGetIndex(model.IndexType, out var index))
            {
                if (string.IsNullOrEmpty(model.SearchTerm))
                {
                    throw new ArgumentException("Search term is required");
                }
                model.SearchTerm = CleanseSearchTerm(model.SearchTerm);

                // Tokenize the search term
                model.SearchTerms = Tokenize(model.SearchTerm);
                var searcher = index.GetSearcher();
                var criteria = searcher.CreateQuery();
                if (model.SearchFields.Contains("umbracoFile") && !model.SearchFields.Contains("umbracoFileName"))
                {
                    model.SearchFields.Add("umbracoFileName");
                }
                IBooleanOperation booleanOperation = model.FuzzySearch ? criteria.Field(model.SearchFields.FirstOrDefault(), model.SearchTerms.FirstOrDefault().Fuzzy()) : criteria.Field(model.SearchFields.FirstOrDefault(), model.SearchTerms.FirstOrDefault());
                foreach (var term in model.SearchTerms.Skip(1))
                {
                    foreach (var searchField in model.SearchFields)
                    {
                        booleanOperation.Or().Field(searchField, term);
                    }
                }

                if (!string.IsNullOrEmpty(model.HideFromSearchField))
                {
                    booleanOperation.Not().Field(model.HideFromSearchField, 1);
                }
                if (model.RootContentNodeId > 0)
                {
                    booleanOperation.And().Field("searchPath", model.RootContentNodeId.ToString());
                    booleanOperation.And().Field("__indexType", "content");
                }
                else if(model.RootMediaNodeId > 0)
                {
                    booleanOperation.And().Field("__indexType", "media");
                }

                if (model.IgnoreDocumentTypes != null && model.IgnoreDocumentTypes.Any())
                {
                    booleanOperation.Not().GroupedAnd(new string[] { "__NodeTypeAlias" }, (string[])model.IgnoreDocumentTypes);
                }
                if (model.OnlyDocumentTypes != null && model.OnlyDocumentTypes.Any())
                {
                    booleanOperation.And().GroupedOr(new string[] { "__NodeTypeAlias" }, (string[])model.OnlyDocumentTypes);
                }
                // Ensure page contains all search terms in some way
                if (model.SearchFilters != null)
                {
                    foreach (var term in model.SearchFilters)
                    {
                        booleanOperation.And().GroupedOr(new string[] { term.Key }, term.Value.ToArray());
                    }
                }
                var results = booleanOperation.Execute(model.PageSize > 0 ? 500 : model.PageSize);
                return results;
            }
            return null;
        }

        // Cleanse the search term
        private string CleanseSearchTerm(string input)
        {
            return input;
        }

        // Splits a string on space, except where enclosed in quotes
        private IEnumerable<string> Tokenize(string input)
        {
            return Regex.Matches(input, @"[\""].+?[\""]|[^ ]+")
                .Cast<Match>()
                .Select(m => m.Value.Trim('\"'))
                .ToList();
        }
    }
}
