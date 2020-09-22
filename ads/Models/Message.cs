using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using System.Web.Mvc;

namespace ads.Models
{

    public partial class Message
    {
        [Display(Name = "Текст сообщение")]
        [StringLength(255)]
        [UIHint("MultilineText")]
        public string text { get; set; }

        [Display(Name = "Дата")]
        public DateTime? datetime { get; set; }
        
        public bool? isRead { get; set; }

        [StringLength(128)]
        public string idSender { get; set; }

        [HiddenInput(DisplayValue = false)]
        public int id { get; set; }

        public virtual ApplicationUser UserSender { get; set; }

        public int idChat { get; set; }

        public virtual Chat Chat { get; set; }

    }
}
