using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Mockify.ViewModels {
    public class Paging<T> {

        public Type Proto { get; set; } = typeof(T);

        public int Total { get; set; } = 1;

        [Required]
        public int Offset { get; set; } = 0;

        [Required]
        public int Limit { get; set; } = 25;

        [Required]
        public IList<T> Items { get; set; } = new List<T>();


        public int TotalPages {
            get {
                if (Total == 0) { return 1; }
                return (int)Math.Ceiling((double)Total / Limit);
            }
        }

        public int CurrentPage {
            get {
                return (int)Math.Ceiling((double)Offset / Limit) + 1;
            }
        }


    }
}
