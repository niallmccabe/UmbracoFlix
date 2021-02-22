using MiniflixApp.Core.Interfaces;
using MiniflixApp.Core.Services;
using Umbraco.Core;
using Umbraco.Core.Composing;

namespace MiniflixApp.Web.Composers
{
    public class RegisterDependencyComposition : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.Register<ISearchService, ExamineSearchService>();
        }
    }
}