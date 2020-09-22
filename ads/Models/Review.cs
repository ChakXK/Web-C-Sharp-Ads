using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using System.Web.Mvc;

namespace ads.Models
{
    public class Review
    {
        [Display(Name = "Текст отзыва")]
        [UIHint("MultilineText")]
        [StringLength(255)]
        public string text { get; set; }

        [Display(Name = "Дата")]
        public DateTime? datetime { get; set; }

        public bool? isRead { get; set; }

        [StringLength(128)]
        public string idSender { get; set; }

        [StringLength(128)]
        public string idRecipient { get; set; }

        [HiddenInput(DisplayValue = false)]
        public int id { get; set; }

        public virtual ApplicationUser UserSender { get; set; }

        public virtual ApplicationUser UserRecipient { get; set; }
    }
}