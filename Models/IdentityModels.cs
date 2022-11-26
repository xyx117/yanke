using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace duandian_test.Models
{
    // 可以通过向 ApplicationUser 类添加更多属性来为用户添加配置文件数据。若要了解详细信息，请访问 http://go.microsoft.com/fwlink/?LinkID=317594。
    public class ApplicationUser : IdentityUser
    {
        //用户信息自定义字段
        public string role { get; set; }


        //所属科室
        public string keshi { get; set; }


        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // 请注意，authenticationType 必须与 CookieAuthenticationOptions.AuthenticationType 中定义的相应项匹配
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // 在此处添加自定义用户声明
            return userIdentity;
        }
    }

    //连接数据库 用户部分的  上下文段
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("IdentityDb", throwIfV1Schema: false)
        {
        }

        //这段代码是新增的
        static ApplicationDbContext()
        {
            Database.SetInitializer<ApplicationDbContext>(new IdentityDbInit());
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }


    //这段代码是新增的
    public class IdentityDbInit : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            PerformInitialSetup(context);
            base.Seed(context);
        }
        public void PerformInitialSetup(ApplicationDbContext context)
        {
            //初始化

            ApplicationUserManager userMgr = new ApplicationUserManager(new UserStore<ApplicationUser>(context));
            AppRoleManager roleMgr = new AppRoleManager(new RoleStore<AppRole>(context));

            //int[] numbers = new int[]{1,2,3,4,5,6};
            string[] roleNames = new string[] { "管理员账号","分账号" };

            for (int i = 0; i < roleNames.Length; i++)
            {
                if (!roleMgr.RoleExists(roleNames[i]))
                {
                    roleMgr.Create(new AppRole(roleNames[i]));
                }
            }
            //string roleName = "Administrators";

            string userName = "admin";
            string password = "123456";
            string email = "admin@example.com";

            ApplicationUser user = userMgr.FindByName(userName);
            if (user == null)
            {

                //UserName = model.name, suoshuxueyuan = xueyuan, Email = model.name + "@qq.com", zhenshiname = model.zhenshiname, role = model.role, parentID = -1, usercount = 0
                userMgr.Create(new ApplicationUser { UserName = userName, Email = email, role = "管理员账号", keshi = "all" },
                    password);
                user = userMgr.FindByName(userName);
            }

            if (!userMgr.IsInRole(user.Id, roleNames[0]))//在这里判断admin是不是在数组中的角色，不是就添加到第三下标的角色
            {
                userMgr.AddToRole(user.Id, roleNames[0]);
            }

            //foreach (ApplicationUser dbUser in userMgr.Users)
            //{
            //    dbUser.City = Cities.PARIS;
            //}
            context.SaveChanges();

        }
    }

}