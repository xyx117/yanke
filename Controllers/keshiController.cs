using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using duandian_test.DAL;
using duandian_test.Models;
using System.Data;
using System.Data.Entity;

namespace duandian_test.Controllers
{
    public class keshiController : Controller
    {
        private sbglContent db = new sbglContent();

        // GET: keshi
        public ActionResult Index()
        {
            var kehsi = from m in db.keshis
                         select m;
            return View(kehsi);
        }

        [Authorize(Roles = "管理员账号")]
        public ActionResult create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "管理员账号")]
        [Authorize]
        [ValidateAntiForgeryToken]
        //   public async Task<ActionResult> Create([Bind(Include = "xiangmuID,mingcheng,xiangmudalei,zijinlaiyuan,lierulanmu,wenjianbianhao,zhixingdanwei,lianxiren,jine,xiadariqi,zijinxingzhi,xiabobumen,lururen,shenheren")] xiangmuzongbiao xiangmuzongbiao)
        public ActionResult Create([Bind(Include = "shiyongkeshi")] keshi keshi)//
        {         
            try
            {
                if (ModelState.IsValid)
                {                    
                    db.keshis.Add(keshi);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException e)
            {
                ModelState.AddModelError("", e.ToString());                
            }
            return View();    //返回错误描述
        }

        [Authorize(Roles = "管理员账号")]
        public ActionResult Edit(int xuhao)
        {            
            keshi keshi = db.keshis.Where(s => s.xuhao == xuhao).FirstOrDefault();

            if (keshi == null)
            {
                return HttpNotFound();
            }
            return View(keshi);
        }

        [HttpPost]
        [Authorize(Roles = "管理员账号")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "xuhao,shiyongkeshi")] keshi keshi)
        {
            if (ModelState.IsValid)
            {                
                db.Entry(keshi).State = EntityState.Modified;
                
                db.SaveChanges();
                return RedirectToAction("Index", "keshi");
            }
            return View();
        }

        
        [Authorize(Roles = "管理员账号")]
        public ActionResult Delete(int xuhao)
        {           
            keshi keshi = db.keshis.Where(s => s.xuhao == xuhao).FirstOrDefault();

            if (keshi == null)
            {
                return HttpNotFound();
            }
            return View(keshi);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "管理员账号")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int xuhao)
        {
            //xiangmuzongbiao xiangmuzongbiao = await db.xiangmuzongbiaos.FindAsync(id);
            keshi keshi = db.keshis.Where(s => s.xuhao == xuhao).FirstOrDefault();
            db.keshis.Remove(keshi);
            db.SaveChanges();
            return RedirectToAction("Index", "keshi");
        }
    }
}