using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.IO;

namespace MiniflixApp.Web.Composers
{
    public class SetMediaFileSystemComposer : IUserComposer
    {
        public void Compose(Umbraco.Core.Composing.Composition composition)
        {
            //composition.SetMediaFileSystem(() => new PhysicalFileSystem(@"F:\Storage\UmbracoMedia", "/media"));
        }
    }
}