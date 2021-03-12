using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MiniflixApp.Core.Models.ViewModels
{
    public class AddMediaItemViewModel
    {
        public string MediaType { get; set; }
        public string Name { get; set; }

        [DataType(DataType.Upload)]
        public HttpPostedFileBase File { get; set; }
    }
}
