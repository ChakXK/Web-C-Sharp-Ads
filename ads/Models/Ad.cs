using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using System.Web.Mvc;

namespace ads.Models
{

    public partial class Ad
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Ad()
        {
            Chats = new HashSet<Chat>();
            Images = new HashSet<Image>();
        }

        [HiddenInput(DisplayValue = false)]
        public int id { get; set; }

        [Display(Name = "Заголовок")]
        [StringLength(50, MinimumLength = 3)]
        public string title { get; set; }

        [Display(Name = "Описание")]
        [StringLength(255)]
        public string text { get; set; }

        [Display(Name = "Цена")]
        [Range(minimum:0,maximum:int.MaxValue)]
        public int prise { get; set; }
      
        [Display(Name = "Подано")]
        public DateTime? datetime { get; set; }

        [Display(Name = "Категория")]
        public int? idSubject { get; set; }

        [StringLength(128)]
        public string idUser { get; set; }

        public int? idImage { get; set; }

        public int? idStatusAd { get; set; }

        public int? idCity { get; set; }

        public virtual City City { get; set; }
        [Display(Name = "Категория")]
        public virtual Subject Subject { get; set; }

        public virtual ApplicationUser User { get; set; }

        public virtual Image Image { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Chat> Chats { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Image> Images { get; set; }

        public virtual AdStatus AdStatus { get; set; }
    }
}
