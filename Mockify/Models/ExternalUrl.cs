//using Microsoft.EntityFrameworkCore.comp

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mockify.Models {

    public class ExternalUrl {

        [Key]
        public string ExternalUrlId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
