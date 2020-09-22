using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using System.Web.Mvc;

namespace ads.Models
{
    public class ReviewsAboutViewModel
    {
       public List<Review> Reviews;
        public ApplicationUser Recipient;
    }
}