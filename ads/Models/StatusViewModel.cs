using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace ads.Models
{
    public class StatusViewModel
    {
        public string Name;
        public int Id;
        public int Count;
    }
}