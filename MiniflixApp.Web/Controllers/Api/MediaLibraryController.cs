using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Web.WebApi;

namespace MiniflixApp.Web.Controllers.Api
{
    public class MediaLibraryController : UmbracoApiController
    {
        [HttpPost]
        [AllowAnonymous]
        public async Task<HttpResponseMessage> UploadFileChunks([FromBody] HttpPostedFileBase file, bool lastChunk = false)
        {

            if (file != null)
            {
                try
                {
                    string filePath = Path.Combine(GetUploadPath(), file.FileName);

                    using (FileStream fs = new FileStream(filePath, FileMode.Append))
                    {
                        var bytes = GetBytes(file.InputStream);
                        fs.Write(bytes, 0, bytes.Length);
                    }
                    if (lastChunk)
                    {
                        try
                        {
                            using(FileStream fileStream = new FileStream(filePath, FileMode.Open))
                            {
                                var contentTypeBaseServiceProvider = Services.ContentTypeBaseServices;
                                IMedia media = Services.MediaService.CreateMedia(file.FileName, 1099, Constants.Conventions.MediaTypes.File);
                                media.SetValue(contentTypeBaseServiceProvider, "umbracoFile", file.FileName, fileStream);
                                Services.MediaService.Save(media);
                            }                            
                        }
                        catch (Exception e)
                        {
                            Logger.Error(typeof(MediaLibraryController), e, "InsertFile | Exception: {0} | Message: {1}", e.InnerException != null ? e.InnerException.ToString() : "", e.Message != null ? e.Message.ToString() : "");
                        }
                    }

                    return Request.CreateResponse(HttpStatusCode.OK, new { status = true });//media;
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = false, message = ex.Message });
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { status = false });
        }

        private byte[] GetBytes(Stream input)
        {
            byte[] buffer = new byte[input.Length];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
        {
                    ms.Write(buffer, 0, read);
                }

                return ms.ToArray();
            }
        }

        private string GetUploadPath()
        {
            var rootPath = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/UploadedFiles/");

            if (!Directory.Exists(rootPath))
            {
                Directory.CreateDirectory(rootPath);
            }

            return rootPath;
        }
    }
}