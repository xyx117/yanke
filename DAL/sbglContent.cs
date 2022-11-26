using duandian_test.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace duandian_test.DAL
{
    public class sbglContent : DbContext
    {
        public sbglContent()
            : base("sbglContent")
        {
            this.Configuration.ProxyCreationEnabled = false;     //关闭关联的子表
        }

        //设备表
        public DbSet<shebei> shebeis { get; set; }

        //修改日志表
        public DbSet<xiugairizhi> xiugairizhis { get; set; }

        public DbSet<keshi> keshis { get; set; }


        public DbSet<fenlei> fenleis { get; set; }


        public DbSet<danwei> danweis { get; set; }


        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{

        //    modelBuilder.Entity<xiangmuzongbiao>().MapToStoredProcedures();

        //    modelBuilder.Entity<zhixingdanwei>().MapToStoredProcedures();


        //}
    }
}