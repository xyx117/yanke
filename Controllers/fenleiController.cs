using duandian_test.DAL;
using duandian_test.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace duandian_test.Controllers
{
    public class fenleiController : Controller
    {
        private sbglContent db = new sbglContent();

        // GET: keshi
        public ActionResult Index()
        {
            var fenlei = from m in db.fenleis
                        select m;
            return View(fenlei);
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

        public ActionResult Create([Bind(Include = "shiyongfenlei")] fenlei fenlei)//
        {
            try
            {
                if (ModelState.IsValid)
                {

                    db.fenleis.Add(fenlei);
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
            fenlei fenlei = db.fenleis.Where(s => s.xuhao == xuhao).FirstOrDefault();

            if (fenlei == null)
            {
                return HttpNotFound();
            }
            return View(fenlei);
        }      

        [HttpPost]
        [Authorize(Roles = "管理员账号")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "xuhao,shiyongfenlei")] fenlei fenlei)
        {
            if (ModelState.IsValid)
            {
                db.Entry(fenlei).State = EntityState.Modified;

                db.SaveChanges();
                return RedirectToAction("Index", "fenlei");
            }
            return View();
        }
               

        [Authorize(Roles = "管理员账号")]
        public ActionResult Delete(int xuhao)
        {
            fenlei fenlei = db.fenleis.Where(s => s.xuhao == xuhao).FirstOrDefault();

            if (fenlei == null)
            {
                return HttpNotFound();
            }
            return View(fenlei);
        }


        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "管理员账号")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int xuhao)
        {
            //xiangmuzongbiao xiangmuzongbiao = await db.xiangmuzongbiaos.FindAsync(id);
            fenlei fenlei = db.fenleis.Where(s => s.xuhao == xuhao).FirstOrDefault();
            db.fenleis.Remove(fenlei);
            db.SaveChanges();
            return RedirectToAction("Index", "fenlei");

        }


    }
}