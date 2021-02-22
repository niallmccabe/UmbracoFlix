using Examine;
using Examine.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.Scoping;
using Umbraco.Examine;
using Umbraco.Web;

namespace MiniflixApp.Core.Events
{
    public class ExamineComposer : ComponentComposer<ExamineEvents>, IUserComposer
    {
    }
    public class ExamineEvents : IComponent
    {
        private readonly IExamineManager _examineManager;
        private readonly IUmbracoContextFactory _umbracoContextFactory;
        private readonly IScopeProvider _scopeProvider;

        public ExamineEvents(IExamineManager examineManager, IUmbracoContextFactory umbracoContextFactory, IScopeProvider scopeProvider)
        {
            _examineManager = examineManager;
            _umbracoContextFactory = umbracoContextFactory;
            _scopeProvider = scopeProvider;
        }

        public void Initialize()
        {
            if (!_examineManager.TryGetIndex(Constants.UmbracoIndexes.ExternalIndexName, out IIndex index))
                throw new InvalidOperationException($"No index found by name {Constants.UmbracoIndexes.ExternalIndexName}");

            //we need to cast because BaseIndexProvider contains the TransformingIndexValues event
            if (!(index is BaseIndexProvider indexProvider))
                throw new InvalidOperationException("Could not cast");

            indexProvider.TransformingIndexValues += IndexProviderTransformingIndexValues;
        }

        private void IndexProviderTransformingIndexValues(object sender, IndexingItemEventArgs e)
        {
            if (e.ValueSet.Category == IndexTypes.Content)
            {
                var combinedFields = new StringBuilder();

                var pathBuilder = new StringBuilder();

                foreach (var fieldValues in e.ValueSet.Values)
                {
                    foreach (var value in fieldValues.Value)
                    {
                        if (value != null)
                        {
                            combinedFields.AppendLine(value.ToString());
                        }
                    }
                    if (fieldValues.Key == "path")
                    {
                        foreach (var value in fieldValues.Value)
                        {
                            if (value != null)
                            {
                                var path = value.ToString().Replace(",", " ");
                                pathBuilder.Append(path);
                            }
                        }
                    }
                }

                

                //Accessing the Umbraco Cache code will be added in the next step.
                //combinedFields.AppendLine(GetBreadcrumb(e.ValueSet.Values["id"].FirstOrDefault()?.ToString()));

                e.ValueSet.TryAdd("combinedField", combinedFields.ToString());

                e.ValueSet.TryAdd("searchPath", pathBuilder.ToString());

            }
        }

        public void Terminate()
        {
            if (!_examineManager.TryGetIndex(Constants.UmbracoIndexes.ExternalIndexName, out IIndex index))
                throw new InvalidOperationException($"No index found by name {Constants.UmbracoIndexes.ExternalIndexName}");

            //we need to cast because BaseIndexProvider contains the TransformingIndexValues event
            if (!(index is BaseIndexProvider indexProvider))
                throw new InvalidOperationException("Could not cast");
            //unsubscribe during shutdown
            indexProvider.TransformingIndexValues -= IndexProviderTransformingIndexValues;
        }
    }
}
