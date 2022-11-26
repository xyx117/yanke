using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace duandian_test.Models
{
    public class shebei
    {
        //序号
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //[Required(ErrorMessage = "序号不能为空")]
        [Display(Name = "序号")]
        public int xuhao { get; set; }


        //设备编号
        [Display(Name = "编号")]
        public string bianhao { get; set; }


        //设备，物资名称
        [Display(Name = "名称")]
        [Required(ErrorMessage = "设备名称不能为空")]
        public string shiyongshebei { get; set; }


        //规格型号
        [Display(Name = "型号")]
        public string guigexinghao { get; set; }


        //单位
        [Display(Name = "单位")]
        public string danwei { get; set; }


        //数量
        [Display(Name = "数量")]
        public int? shuliang { get; set; }

        //价格
        [Display(Name = "价格")]
        public decimal? jiage { get; set; }


        //发票
        [Display(Name = "发票")]
        public string fapiao { get; set; }


        //生产厂家
        [Display(Name = "厂家")]
        public string changjia { get; set; }


        //销售厂商
        [Display(Name = "销售商")]
        public string xiaoshoushang { get; set; }


        //使用时间
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "使用时间")]
        public DateTime? shiyongshijian { get; set; }


        //放置地点
        [Display(Name = "放置点")]
        public string fangzhididian { get; set; }


        //分类    分成医疗设备、办公用品、生活用品、其他物品4大类
        [Display(Name = "分类")]
        public string fenlei { get; set; }


        //状态
        [Display(Name = "状态")]
        public string zhuangtai { get; set; }


        //调配
        [Display(Name = "调配")]
        public string diaopei { get; set; }


        //负责人电话
        [Display(Name = "负责人")]
        public string fuzerendianhua { get; set; }


        //工程师电话
        [Display(Name = "工程师")]
        public string gongchengshidianhua { get; set; }

        //备注
        [Display(Name = "备注")]
        public string beizhu { get; set; }


        //科室
        [Display(Name = "科室")]
        [Required]
        public string keshi { get; set; }


        //审核状态
        [Display(Name = "审核状态")]
        public string shenhezhuangtai { get; set; }


        [StringLength(10, ErrorMessage = "不能超过5个汉字。")]
        [Display(Name = "录入日期")]
        public string lururiqi { get; set; }


        //因为序号自增长后，在日志中已经无法再创建记录的时候记录日志
        public string lururen { get; set; }


        //这里和修改日志表是 一对多的关系
        public virtual ICollection<shebei> xiugairizhis { get; set; }
    }
}