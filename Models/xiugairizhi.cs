using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace duandian_test.Models
{
    public class xiugairizhi
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int xiugairizhiID { get; set; }

        public int shebeiID { get; set; }

        //操作
        public string caozuo { get; set; }

        //操作人
        public string caozuoren { get; set; }

        //操作日期
        public string caozuoriqi { get; set; }


        public virtual shebei shebei { get; set; }
    }
}