using MiniflixApp.Core.Interfaces;
using MiniflixApp.Core.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Umbraco.Web;
using Umbraco.Web.WebApi;
using Constants = Umbraco.Core.Constants;

namespace MiniflixApp.Web.Controllers.Api
{
    public class SearchController : UmbracoApiController
    {
        private readonly ISearchService _searchService;
        public SearchController(ISearchService searchService)
        {
            _searchService = searchService;
        }
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage Autocomplete(string searchTerm)
        {
            var rootNode = Umbraco.ContentAtRoot().FirstOrDefault();
            var results = _searchService.GetAutoCompleteResults(new SearchModel
            {
                SearchTerm = searchTerm,
                SearchFields = new List<string> { "nodeName", "genres", "categories"},
                IndexType = Constants.UmbracoIndexes.ExternalIndexName,
                FuzzySearch = true,
                RootContentNodeId = rootNode.Id
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
            return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(searchResults));
        }

        public IEnumerable<SearchResultModel> Get(SearchModel searchModel)
        {
            var results = _searchService.GetResults(searchModel);

            var searchResults = results?.Select(x => {
                var node = Umbraco.Content(x.Id);
                return new SearchResultModel
                {
                    Description = x.Values["description"],
                    Name = x.Values["nodeName"],
                    LinkUrl = node.Url()
                };
            });
            return searchResults;
        }

        // POST api/<controller>
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}