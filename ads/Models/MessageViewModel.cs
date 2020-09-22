using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using System.Web.Mvc;

namespace ads.Models
{
    public class DialogViewModel
    {
        public Chat Chatt;
        public int CountMess;
        public List<ChatUser> Chaters;
    }
    public class MessageToAdminViewModel
    {
        [Display(Name = "Текст сообщение")]
        [StringLength(255)]
        [UIHint("MultilineText")]
        public string text { get; set; }

        [Display(Name = "Тема")]
        [StringLength(128)]
        public string title { get; set; }

    }
}