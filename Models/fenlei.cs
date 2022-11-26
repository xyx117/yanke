using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace duandian_test.Models
{
    public class fenlei
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "序号")]
        public int xuhao { get; set; }


        [Display(Name = "序号")]
        public string shiyongfenlei { get; set; }
    }
}