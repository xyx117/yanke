using Aspose.Cells;
using duandian_test.DAL;
using duandian_test.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Transactions;
using System.Configuration;
using PagedList;

namespace duandian_test.Controllers
{
    public class HomeController : Controller
    {
        private sbglContent db = new sbglContent();


        //要可以根据  审核状态，科室，报废、在用状态筛选
        public ActionResult Index(int? page, string sortOrder, string searchString, string zhuangtai, string shenhejieguo)
        {
            var shebei = from m in db.shebeis
                         orderby m.xuhao descending
                         select m;

            if (shenhejieguo == null || shenhejieguo == "全部")
            {
                ViewBag.shenhejieguozhi = "全部";
            }
            else
            {
                shebei = shebei.Where(s => s.shenhezhuangtai == shenhejieguo).OrderByDescending(s=>s.xuhao);
                ViewBag.shenhejieguozhi = shenhejieguo;
            }

            if (zhuangtai == null || zhuangtai == "全部")
            {

                ViewBag.zhuangtai = zhuangtai;
            }
            else
            {
                shebei = shebei.Where(s => s.zhuangtai == zhuangtai).OrderByDescending(s => s.xuhao);
                ViewBag.zhuangtai = zhuangtai;
            }

            //按照项目名称和下达日期进行升序或降序排列
            ViewBag.xiangmuleibie = String.IsNullOrEmpty(sortOrder) ? "Name_desc" : "";
            ViewBag.xiadariqi = sortOrder == "Date" ? "Date_desc" : "Date";

            //模糊查询
            if (!String.IsNullOrEmpty(searchString))
            {              
                shebei = shebei.Where(s => s.shiyongshebei.ToUpper().Contains(searchString.ToUpper())
                    || s.keshi == searchString).OrderByDescending(s => s.xuhao);                
            }

            ViewBag.searchzhi = searchString;

            ViewBag.sortOrderzhi = sortOrder;

            switch (sortOrder)
            {
                case "Name_desc":
                    shebei = shebei.OrderBy(s => s.xuhao);
                    break;
                case "Date":
                    shebei = shebei.OrderBy(s => s.lururiqi);
                    break;
                case "Date_desc":
                    shebei = shebei.OrderByDescending(s => s.lururiqi);
                    break;
                default:
                    
                    shebei = shebei.OrderByDescending(s => s.xuhao);
                    break;
            }
            int pagesize = 100;
            int pagenumber = (page ?? 1);
            return View(shebei.ToPagedList(pagenumber, pagesize));            
        }      
        
        //[Authorize(Roles = "管理员账号,分账号")]
        [Authorize]
        public ActionResult Create(string username)
        {

            var GenreLst = new List<string>();
            GenreLst.Add("");
            var GenreQry = from d in db.fenleis
                           orderby d.xuhao
                           select d.shiyongfenlei;
            GenreLst.AddRange(GenreQry.Distinct());
            
            //ViewBag.zxdanwei = new SelectList(GenreLst);
            ViewBag.fenlei = GenreLst;



            var keshi = new List<string>();
            keshi.Add("");
            var keshi_q = from d in db.keshis
                           orderby d.xuhao
                           select d.shiyongkeshi;
            keshi.AddRange(keshi_q.Distinct());      
            ViewBag.keshi = keshi;


            var danwei = new List<string>();
            danwei.Add("");
            var danwei_q = from d in db.danweis
                          orderby d.xuhao
                          select d.shiyongdanwei;
            danwei.AddRange(danwei_q.Distinct());
            ViewBag.danwei = danwei;


            ViewBag.username = username;
            return View();
        }


        // POST: Home/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        //[Authorize(Roles = "管理员账号,分账号")]
        [Authorize]
        [ValidateAntiForgeryToken]
        //   public async Task<ActionResult> Create([Bind(Include = "xiangmuID,mingcheng,xiangmudalei,zijinlaiyuan,lierulanmu,wenjianbianhao,zhixingdanwei,lianxiren,jine,xiadariqi,zijinxingzhi,xiabobumen,lururen,shenheren")] xiangmuzongbiao xiangmuzongbiao)
        public ActionResult Create([Bind(Include = "bianhao,shiyongshebei,guigexinghao,danwei,shuliang,fapiao,changjia,xiaoshoushang,shiyongshijian,fangzhididian,fenlei,zhuangtai,diaopei,fuzerendianhua,gongchengshidianhua,keshi,beizhu")] shebei shebei)//
        {
            if (User.IsInRole("分账号"))
            {
                shebei.shenhezhuangtai = "未审核";
            }
            else
            {
                shebei.shenhezhuangtai = "通过";
            }
            shebei.jiage = Convert.ToDecimal( Request.Form["jiage"]);
            shebei.lururiqi = DateTime.Now.ToString("yyyy-MM-dd");//yyyy-MM-dd HH：mm：ss：ffff
            shebei.lururen = Request.Form["username"];           

            if (ModelState.IsValid)
            {                
                try
                {
                    db.shebeis.Add(shebei);

                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "不能保存，序号重复！" + ex.ToString());
                }                   
            }
            return View();    //返回错误描述
        }


        [Authorize]
        public ActionResult Details(int id, string sortOrder, string searchstring, string shenhejieguo, string zhuangtai)
        {
            ViewBag.zhuangtai = zhuangtai;
            ViewBag.searchzhi = searchstring;
            ViewBag.shenhejieguozhi = shenhejieguo;
            ViewBag.sortOrderzhi = sortOrder;
            shebei shebei = db.shebeis.Where(s => s.xuhao == id).FirstOrDefault();

            if (shebei == null)
            {
                return HttpNotFound();
            }
            return View(shebei);
        }

        
        //日志
        [Authorize]
        public ActionResult rizhi(int id, string sortOrder, string searchstring, string shenhejieguo, string zhuangtai)
        {

            ViewBag.zhuangtai = zhuangtai;
            ViewBag.searchzhi = searchstring;
            ViewBag.shenhejieguozhi = shenhejieguo;
            ViewBag.sortOrderzhi = sortOrder;           
            var rizhi = db.xiugairizhis.Where(s => s.shebeiID == id);

            if (rizhi == null)
            {
                return HttpNotFound();
            }
            return View(rizhi);
        }

        // GET: Home/Delete/5
        [Authorize(Roles = "管理员账号")]
        public ActionResult Delete(int id, string shenhezhuangtai, string sortOrder, string searchString, string shenhejieguo, string zhuangtai)
        {
            ViewBag.zhuangtai = zhuangtai;
            ViewBag.searchzhi = searchString;
            ViewBag.shenhejieguozhi = shenhejieguo;
            ViewBag.sortOrderzhi = sortOrder;
            
            shebei shebei = db.shebeis.Where(s => s.xuhao == id).FirstOrDefault();
            
            return View(shebei);
            //return RedirectToAction("Index", "Home", new { sortOrder = sortOrder, searchString = searchString, niandu = niandu, shenhejieguo = shenhejieguo });
        }


        // POST: Home/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "管理员账号")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, string zhuangtai, string searchzhi, string shenhejieguozhi, string sortOrderzhi)
        {
            //xiangmuzongbiao xiangmuzongbiao = await db.xiangmuzongbiaos.FindAsync(id);
            shebei shebei = db.shebeis.Where(s => s.xuhao == id).FirstOrDefault();
            db.shebeis.Remove(shebei);
            db.SaveChanges();
            return RedirectToAction("Index", "Home", new { sortOrder = sortOrderzhi, searchString = searchzhi, zhuangtai = zhuangtai, shenhejieguo = shenhejieguozhi });
        }



        // GET: Home/shenhe/5
        [Authorize(Roles = "管理员账号")]
        public ActionResult shenhe(int id, string sortOrder, string searchString, string shenhejieguo, string zhuangtai, string username)
        {
            ViewBag.zhuangtai = zhuangtai;
            ViewBag.searchzhi = searchString;
            ViewBag.shenhejieguozhi = shenhejieguo;
            ViewBag.sortOrderzhi = sortOrder;
            ViewBag.username = username;
            
            shebei shebei = db.shebeis.Where(s => s.xuhao == id).FirstOrDefault();
            if (shebei == null)
            {
                return HttpNotFound();
            }

            return View(shebei);
        }

        // POST: Home/shenhe/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [Authorize(Roles = "管理员账号")]
        [ValidateAntiForgeryToken]
        //public async Task<ActionResult> shenhe([Bind(Include = "ID,niandu,xiangmuleibie,mingcheng,shangcaixiangmuhao,lierulanmu,bianhao,zhixingdanwei,lianxiren,jine,xiangmufenlei,zijinlaiyuan,xiangmulaiyuan,zijinxingzhi,xiabobumen,xiadariqi,xiadaqingkuang,lururen,lururiqi,shenheren,shenheriqi,shenhezhuangtai,xiaozhanghui,dangweihui,beizhu")] xiangmuzongbiao xiangmuzongbiao)
        public ActionResult shenhe(string zhuangtai, string searchzhi, string shenhejieguozhi, string sortOrderzhi)//, shebei shebei
        {
            int xuhao = Convert.ToInt32(Request.Form["xuhao"]);

            shebei shebei = db.shebeis.Where(s => s.xuhao == xuhao).FirstOrDefault();

            shebei.beizhu = Request.Form["beizhu"];

            shebei.shenhezhuangtai = Request.Form["shenhezhuangtai"];


            xiugairizhi rizhi = new xiugairizhi();

            rizhi.shebeiID = shebei.xuhao;

            rizhi.caozuo = "审核";

            rizhi.caozuoren = Request.Form["username"];

            rizhi.caozuoriqi = DateTime.Now.ToString("yyyy-MM-dd");


            if (ModelState.IsValid)
            {
                db.Entry(shebei).State = EntityState.Modified;
                db.xiugairizhis.Add(rizhi);
                db.SaveChanges();
                return RedirectToAction("Index", "Home", new { sortOrder = sortOrderzhi, searchString = searchzhi, zhuangtai = zhuangtai, shenhejieguo = shenhejieguozhi });
            }
            return View(shebei);
        }

        // GET: Home/Edit/5
        //[Authorize(Roles = "管理员账号,分账号")]
        [Authorize]
        public ActionResult Edit(int id, string sortOrder, string searchstring, string shenhejieguo, string zhuangtai,string username)
        {
            ViewBag.zhuangtai = zhuangtai;
            ViewBag.searchzhi = searchstring;
            ViewBag.shenhejieguozhi = shenhejieguo;
            ViewBag.sortOrderzhi = sortOrder;

            ViewBag.username = username;

            //分类数据字典
            var GenreLst = new List<string>();
            GenreLst.Add("");
            var GenreQry = from d in db.fenleis
                           orderby d.xuhao
                           select d.shiyongfenlei;
            GenreLst.AddRange(GenreQry.Distinct());
            ViewBag.fenlei = GenreLst;

            //科室数据字典
            var keshi = new List<string>();
            keshi.Add("");
            var keshi_q = from d in db.keshis
                          orderby d.xuhao
                          select d.shiyongkeshi;
            keshi.AddRange(keshi_q.Distinct());
            ViewBag.keshi = keshi;


            //单位数据字典
            var danwei = new List<string>();
            danwei.Add("");
            var danwei_q = from d in db.danweis
                           orderby d.xuhao
                           select d.shiyongdanwei;
            danwei.AddRange(danwei_q.Distinct());
            ViewBag.danwei = danwei;


            shebei shebei = db.shebeis.Where(s => s.xuhao == id).FirstOrDefault();
            if (shebei == null)
            {
                return HttpNotFound();
            }
            return View(shebei);
        }



        // POST: Home/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        //[Authorize(Roles = "管理员账号,分账号")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string zhuangtai_input, string searchzhi, string shenhejieguozhi, string sortOrderzhi,[Bind(Include = "xuhao,bianhao,shiyongshebei,guigexinghao,danwei,shuliang,jiage,fapiao,changjia,xiaoshoushang,shiyongshijian,fangzhididian,fenlei,zhuangtai,diaopei,fuzerendianhua,gongchengshidianhua,keshi,beizhu,lururiqi")] shebei shebei)//,niandu,xiaozhanghui,dangweihui
        {
            xiugairizhi rizhi = new xiugairizhi();

            rizhi.shebeiID = shebei.xuhao;

            rizhi.caozuo = "修改";

            rizhi.caozuoren = Request.Form["username"];

            rizhi.caozuoriqi = DateTime.Now.ToString("yyyy-MM-dd");


            if (ModelState.IsValid)
            {
                if (User.IsInRole("管理员账号"))
                {
                    shebei.shenhezhuangtai = "通过";
                }
                else
                {
                    shebei.shenhezhuangtai = "未审核";
                }
                db.Entry(shebei).State = EntityState.Modified;
                db.xiugairizhis.Add(rizhi);

                db.SaveChanges();
                return RedirectToAction("Index", "Home", new { sortOrder = sortOrderzhi, searchString = searchzhi, zhuangtai = zhuangtai_input, shenhejieguo = shenhejieguozhi });
            }
            return View();           
        }



        [Authorize(Roles = "管理员账号")]
        public FileResult GetFile()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "Content/uploads/moban/";
            string fileName = "批量导入模板.xlsx";
            return File(path + fileName, "text/plain", fileName);
        }

        [Authorize(Roles = "管理员账号")]
        public ActionResult excelImport(string username)
        {
            ViewBag.username = username;

            return View();
        }


        [HttpPost]
        [Authorize(Roles = "管理员账号")]
        public ActionResult excelImport(FormCollection form)     // 
        {
            ViewBag.error = "";

            string username = Request.Form["username"];


            HttpPostedFileBase fileBase = Request.Files["files"];

            if (fileBase == null || fileBase.ContentLength <= 0)
            {
                ViewBag.tishi = "文件不能为空";

                return View();
            }

            string filename = Path.GetFileName(fileBase.FileName);    //获得文件全名
            //int filesize = file.ContentLength;//获取上传文件的大小单位为字节byte
            string fileEx = System.IO.Path.GetExtension(filename);     //获取上传文件的扩展名
            string NoFileName = System.IO.Path.GetFileNameWithoutExtension(filename);   //获取无扩展名的文件名


            string FileName = NoFileName + DateTime.Now.ToString("yyyyMMddhhmmss") + fileEx;   //这里是一个 文件名+时间+扩展名  的新文件名，作为保存在服务器中的  文件名

            string path = AppDomain.CurrentDomain.BaseDirectory + "content/uploads/";  // 这里获得的是文件的物理路径;
            string savePath = Path.Combine(path, FileName);
            fileBase.SaveAs(savePath);

            Workbook BuildReport_WorkBook = new Workbook();

            //BuildReport_WorkBook.Open(savePath);//fileFullName

            BuildReport_WorkBook.Open(savePath);

            //Worksheets sheets = BuildReport_WorkBook.Worksheets;

            //试题表
            Worksheet workSheetQuestion = BuildReport_WorkBook.Worksheets["Sheet1"];   //  sheet1           

            Cells cellsQuestion = workSheetQuestion.Cells;    //单元格

            //引用事务机制，出错时，事物回滚
            using (TransactionScope transaction = new TransactionScope())
            {
                if (form["gengxinmoshi"].ToString() == "fugai")
                {
                    string ConString = System.Configuration.ConfigurationManager.ConnectionStrings["sbglContent"].ConnectionString;

                    SqlConnection con = new SqlConnection(ConString);

                    string sqldel = "delete from shebeis";  //这个筛选方式写法是否正确待检验

                    int j;    //提示报错的行

                    try
                    {
                        con.Open();

                        SqlCommand sqlcmddel = new SqlCommand(sqldel, con);

                        sqlcmddel.ExecuteNonQuery();   //删除了现有的名单

                        //试题表 
                        for (int i = 1; i < cellsQuestion.MaxDataRow + 1; i++)      //如果导入的有标题，数据从第二行开始
                        {
                            j = i + 1;

                            try
                            {

                                shebei shebei = new shebei();

                                shebei.shenhezhuangtai = "通过";

                                shebei.lururiqi = DateTime.Now.ToString("yyyy-MM-dd");

                                shebei.lururen = username;

                                //编号
                                if (cellsQuestion[i, 0].StringValue == null || cellsQuestion[i, 0].StringValue == "")
                                {
                                    shebei.bianhao = "";
                                }
                                else
                                {
                                    shebei.bianhao = cellsQuestion[i, 0].StringValue;
                                };

                                //设备，物资名称
                                if (cellsQuestion[i, 1].StringValue == null || cellsQuestion[i, 1].StringValue == "")
                                {
                                    ViewBag.tishi = "第" + j + "行记录插入有误，请认真检查格式后再导入！“设备/物资名称”列为不能为空，建议使用模板上传！";
                                    return View();
                                }
                                else
                                {
                                    shebei.shiyongshebei = cellsQuestion[i, 1].StringValue;
                                }

                                //规格型号
                                if (cellsQuestion[i, 2].StringValue == null || cellsQuestion[i, 2].StringValue == "")
                                {
                                    shebei.guigexinghao = "";
                                }
                                else
                                {
                                    shebei.guigexinghao = cellsQuestion[i, 2].StringValue;
                                };

                                //单位
                                if (cellsQuestion[i, 3].StringValue == null || cellsQuestion[i, 3].StringValue == "")
                                {
                                    shebei.danwei = "";
                                }
                                else
                                {
                                    shebei.danwei = cellsQuestion[i,3].StringValue;
                                };


                                //数量
                                if (cellsQuestion[i, 4].StringValue == null || cellsQuestion[i,4].StringValue == "")
                                {
                                    shebei.shuliang = null;
                                }
                                else
                                {
                                    shebei.shuliang = Convert.ToInt32(cellsQuestion[i, 4].StringValue);
                                };

                                //价格
                                if (cellsQuestion[i, 5].StringValue == null || cellsQuestion[i, 5].StringValue == "")
                                {
                                    shebei.jiage = null;
                                }
                                else
                                {
                                    shebei.jiage = Convert.ToDecimal(cellsQuestion[i, 5].StringValue);
                                };

                                //发票
                                if (cellsQuestion[i, 6].StringValue == null || cellsQuestion[i, 6].StringValue == "")
                                {
                                    shebei.fapiao = "";
                                }
                                else
                                {
                                    shebei.fapiao = cellsQuestion[i, 6].StringValue;
                                };

                                //厂家
                                if (cellsQuestion[i, 7].StringValue == null || cellsQuestion[i,7].StringValue == "")
                                {
                                    shebei.changjia = "";
                                }
                                else
                                {
                                    shebei.changjia = cellsQuestion[i,7].StringValue;
                                };

                                //销售商
                                if (cellsQuestion[i, 8].StringValue == null || cellsQuestion[i, 8].StringValue == "")
                                {
                                    shebei.xiaoshoushang = "";
                                }
                                else
                                {
                                    shebei.xiaoshoushang = cellsQuestion[i, 8].StringValue;
                                };

                                //使用时间
                                if (cellsQuestion[i, 9].StringValue == null || cellsQuestion[i, 9].StringValue == "")
                                {
                                    shebei.shiyongshijian = null;
                                }
                                else
                                {
                                    shebei.shiyongshijian = Convert.ToDateTime(cellsQuestion[i, 9].StringValue);
                                };
                                
                                //放置地点
                                if (cellsQuestion[i, 10].StringValue == null || cellsQuestion[i, 10].StringValue == "")
                                {
                                    shebei.fangzhididian = "";
                                }
                                else
                                {
                                    shebei.fangzhididian = cellsQuestion[i, 10].StringValue;
                                };
                                
                                //分类
                                if (cellsQuestion[i, 11].StringValue == null || cellsQuestion[i, 11].StringValue == "")
                                {
                                    shebei.fenlei = "";
                                }
                                else
                                {
                                    shebei.fenlei = cellsQuestion[i, 11].StringValue;
                                };

                                //状态
                                if (cellsQuestion[i, 1].StringValue == null || cellsQuestion[i, 12].StringValue == "")
                                {
                                    shebei.zhuangtai = "";
                                }
                                else
                                {
                                    shebei.zhuangtai = cellsQuestion[i, 12].StringValue;
                                };

                                //调配
                                if (cellsQuestion[i, 13].StringValue == null || cellsQuestion[i, 13].StringValue == "")
                                {
                                    shebei.diaopei = "";
                                }
                                else
                                {
                                    shebei.diaopei = cellsQuestion[i, 13].StringValue;
                                };

                                //负责人电话
                                if (cellsQuestion[i, 14].StringValue == null || cellsQuestion[i, 14].StringValue == "")
                                {
                                    shebei.fuzerendianhua = "";
                                }
                                else
                                {
                                    shebei.fuzerendianhua = cellsQuestion[i, 14].StringValue;
                                };

                                //工程师电话
                                if (cellsQuestion[i, 15].StringValue == null || cellsQuestion[i, 15].StringValue == "")
                                {
                                    shebei.gongchengshidianhua = "";
                                }
                                else
                                {
                                    shebei.gongchengshidianhua = cellsQuestion[i, 15].StringValue;
                                };

                                //备注
                                if (cellsQuestion[i, 16].StringValue == null || cellsQuestion[i, 16].StringValue == "")
                                {
                                    shebei.beizhu = "";
                                }
                                else
                                {
                                    shebei.beizhu = cellsQuestion[i, 16].StringValue;
                                };

                                //设备，物资名称
                                if (cellsQuestion[i, 17].StringValue == null || cellsQuestion[i, 17].StringValue == "")
                                {
                                    ViewBag.tishi = "第" + j + "行记录插入有误，请认真检查格式后再导入！建议使用模板上传！";
                                    return View();
                                }
                                else
                                {
                                    //科室
                                    shebei.keshi = cellsQuestion[i, 17].StringValue;
                                }
                                //数据库操作
                                db.shebeis.Add(shebei);
                            }
                             //db.SaveChanges();放在这里有问题
                            catch (Exception ex)
                            {
                                string errmsg = ex.ToString();

                                ViewBag.tishi = "第" + j + "行记录插入有误，请认真检查格式后再导入！建议使用模板上传！"+errmsg;

                                return View();
                            }
                        }
                        //db.SaveChanges();      //db.SaveChanges();放在这里有问题
                    }
                    catch (Exception ex)
                    {
                        //ViewBag.tishi = "第" + j + "行记录插入有误，请认真检查格式后再导入！“科室”列为不能为空，建议使用模板上传！";

                        ViewBag.tishi = ex.Message.ToString();

                        return View();
                    }
                    finally
                    {
                        con.Close(); //无论如何都要执行的语句。
                    }

                    db.SaveChanges();
                    // ViewBag.fugai = "离开覆盖了";
                }
                else  //追加模式
                {
                    int j;    //提示报错的行
                    try
                    {
                        //试题表 
                        for (int i = 1; i < cellsQuestion.MaxRow + 1; i++)
                        {
                            j = i + 1;
                            try
                            {
                                shebei shebei = new shebei();

                                shebei.shenhezhuangtai = "通过";

                                shebei.lururiqi = DateTime.Now.ToString("yyyy-MM-dd");

                                shebei.lururen = username;                                

                                //编号
                                if (cellsQuestion[i, 0].StringValue == null || cellsQuestion[i, 0].StringValue == "")
                                {
                                    shebei.bianhao = "";
                                }
                                else
                                {
                                    shebei.bianhao = cellsQuestion[i, 0].StringValue;
                                };

                                //设备，物资名称
                                if (cellsQuestion[i, 1].StringValue == null || cellsQuestion[i, 1].StringValue == "")
                                {
                                    ViewBag.tishi = "第" + j + "行记录插入有误，请认真检查格式后再导入！“设备/物资名称”列为不能为空，建议使用模板上传！";
                                    return View();
                                }
                                else
                                {
                                    shebei.shiyongshebei = cellsQuestion[i, 1].StringValue;
                                }

                                //规格型号
                                if (cellsQuestion[i, 2].StringValue == null || cellsQuestion[i, 2].StringValue == "")
                                {
                                    shebei.guigexinghao = "";
                                }
                                else
                                {
                                    shebei.guigexinghao = cellsQuestion[i, 2].StringValue;
                                };

                                //单位
                                if (cellsQuestion[i, 3].StringValue == null || cellsQuestion[i, 3].StringValue == "")
                                {
                                    shebei.danwei = "";
                                }
                                else
                                {
                                    shebei.danwei = cellsQuestion[i, 3].StringValue;
                                };

                                //数量
                                if (cellsQuestion[i, 4].StringValue == null || cellsQuestion[i, 4].StringValue == "")
                                {
                                    shebei.shuliang = null;
                                }
                                else
                                {
                                    shebei.shuliang = Convert.ToInt32(cellsQuestion[i, 4].StringValue);
                                };

                                //价格
                                if (cellsQuestion[i, 5].StringValue == null || cellsQuestion[i, 5].StringValue == "")
                                {
                                    shebei.jiage = null;
                                }
                                else
                                {
                                    shebei.jiage = Convert.ToDecimal(cellsQuestion[i, 5].StringValue);
                                };

                                //发票
                                if (cellsQuestion[i, 6].StringValue == null || cellsQuestion[i, 6].StringValue == "")
                                {
                                    shebei.fapiao = "";
                                }
                                else
                                {
                                    shebei.fapiao = cellsQuestion[i, 6].StringValue;
                                };

                                //厂家
                                if (cellsQuestion[i, 7].StringValue == null || cellsQuestion[i, 7].StringValue == "")
                                {
                                    shebei.changjia = "";
                                }
                                else
                                {
                                    shebei.changjia = cellsQuestion[i, 7].StringValue;
                                };

                                //销售商
                                if (cellsQuestion[i, 8].StringValue == null || cellsQuestion[i, 8].StringValue == "")
                                {
                                    shebei.xiaoshoushang = "";
                                }
                                else
                                {
                                    shebei.xiaoshoushang = cellsQuestion[i, 8].StringValue;
                                };

                                //使用时间
                                if (cellsQuestion[i, 9].StringValue == null || cellsQuestion[i, 9].StringValue == "")
                                {
                                    shebei.shiyongshijian = null;
                                }
                                else
                                {
                                    shebei.shiyongshijian = Convert.ToDateTime(cellsQuestion[i, 9].StringValue);
                                };


                                //放置地点
                                if (cellsQuestion[i, 10].StringValue == null || cellsQuestion[i, 10].StringValue == "")
                                {
                                    shebei.fangzhididian = "";
                                }
                                else
                                {
                                    shebei.fangzhididian = cellsQuestion[i, 10].StringValue;
                                };


                                //分类
                                if (cellsQuestion[i, 11].StringValue == null || cellsQuestion[i, 11].StringValue == "")
                                {
                                    shebei.fenlei = "";
                                }
                                else
                                {
                                    shebei.fenlei = cellsQuestion[i, 11].StringValue;
                                };

                                //状态
                                if (cellsQuestion[i, 1].StringValue == null || cellsQuestion[i, 12].StringValue == "")
                                {
                                    shebei.zhuangtai = "";
                                }
                                else
                                {
                                    shebei.zhuangtai = cellsQuestion[i, 12].StringValue;
                                };

                                //调配
                                if (cellsQuestion[i, 13].StringValue == null || cellsQuestion[i, 13].StringValue == "")
                                {
                                    shebei.diaopei = "";
                                }
                                else
                                {
                                    shebei.diaopei = cellsQuestion[i, 13].StringValue;
                                };

                                //负责人电话
                                if (cellsQuestion[i, 14].StringValue == null || cellsQuestion[i, 14].StringValue == "")
                                {
                                    shebei.fuzerendianhua = "";
                                }
                                else
                                {
                                    shebei.fuzerendianhua = cellsQuestion[i, 14].StringValue;
                                };

                                //工程师电话
                                if (cellsQuestion[i, 15].StringValue == null || cellsQuestion[i, 15].StringValue == "")
                                {
                                    shebei.gongchengshidianhua = "";
                                }
                                else
                                {
                                    shebei.gongchengshidianhua = cellsQuestion[i, 15].StringValue;
                                };

                                //备注
                                if (cellsQuestion[i, 16].StringValue == null || cellsQuestion[i, 16].StringValue == "")
                                {
                                    shebei.beizhu = "";
                                }
                                else
                                {
                                    shebei.beizhu = cellsQuestion[i, 16].StringValue;
                                };

                                //设备，物资名称
                                if (cellsQuestion[i, 17].StringValue == null || cellsQuestion[i, 17].StringValue == "")
                                {
                                    ViewBag.tishi = "第" + j + "行记录插入有误，请认真检查格式后再导入！建议使用模板上传！";
                                    return View();
                                }
                                else
                                {
                                    //科室
                                    shebei.keshi = cellsQuestion[i, 17].StringValue;
                                }
                                //数据库操作
                                db.shebeis.Add(shebei);
                            }

                            catch (Exception ex)
                            {
                                ViewBag.tishi ="第" + j + "行记录插入有误，请认真检查格式后再导入！建议使用模板上传！"+ ex.Message.ToString();

                                return View();
                            }
                        }
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        ViewBag.tishi = ex.Message.ToString();
                        return View();
                    }
                }
                transaction.Complete();
            }
            //string error1 = "祝贺您，本次信息导入成功！";
            //System.Threading.Thread.Sleep(1000);
            ViewBag.tishi = "祝贺您，本次信息导入成功！";
            return View();
        }


        /// <summary>  
        /// 表查询,取得datatable  
        /// </summary>  
        /// <param name="sql"></param>  
        /// <returns>数据表</returns>  
        public static DataTable GetDataTable(string sql)
        {
            string connStr = ConfigurationManager.ConnectionStrings["sbglContent"].ToString();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                DataSet ds = null;
                SqlDataAdapter sda = null;
                
                try
                {
                    ds = new DataSet();

                    sda = new SqlDataAdapter(sql, connStr);

                    sda.Fill(ds);

                    //var count = ds.Tables[0].Rows.Count;   //记录条数

                }
                catch (System.Data.SqlClient.SqlException ex)
                {

                    throw new Exception(ex.Message);
                }
                return ds.Tables[0];
            }
        }       


        //导出分年度汇总表
        //[Authorize(Roles = "管理员账号")]
        public ActionResult exceloutport(string seach)  //这里的id为xiangmuguanli.cs表中的ID
        {
            var sql_keshi = @"select * from [shebeis] where keshi='"+seach+"'";

            var dt_keshi = GetDataTable(sql_keshi);           

            dt_keshi.TableName = "keshi";

            WorkbookDesigner designer = new WorkbookDesigner();
            
            designer.Open(Server.MapPath("~/ykyy_daochu.xlsx"));

            designer.SetDataSource(dt_keshi);

            designer.Process();            

            var byti = designer.Workbook.SaveToStream().GetBuffer();

            designer = null;
            ////Response.End();
                       
            var filename = "***眼科医院固定资产" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xls";
            //转换成流字节，输出浏览器下载

            ////通知浏览器保存文件，其实也就是输出到浏览器
            Response.Clear();
            Response.ContentType = "application/octet-stream";
            Response.AppendHeader("Content-Disposition", "attachment;filename=" + filename);
            Response.BinaryWrite(byti);
            Response.Flush();
            Response.Close();
            return new EmptyResult();
        }
        

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}