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
    public class danweiController : Controller
    {
        private sbglContent db = new sbglContent();

        // GET: keshi
        public ActionResult Index()
        {

            var danwei = from m in db.danweis
                        select m;
            return View(danwei);
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
        public ActionResult Create([Bind(Include = "shiyongdanwei")] danwei danwei)//
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.danweis.Add(danwei);
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
            danwei danwei = db.danweis.Where(s => s.xuhao == xuhao).FirstOrDefault();

            if (danwei == null)
            {
                return HttpNotFound();
            }
            return View(danwei);
        }


        [HttpPost]
        [Authorize(Roles = "管理员账号")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "xuhao,shiyongdanwei")] danwei danwei)
        {
            if (ModelState.IsValid)
            {

                db.Entry(danwei).State = EntityState.Modified;

                db.SaveChanges();
                return RedirectToAction("Index", "danwei");
            }
            return View();
        }


        [Authorize(Roles = "管理员账号")]
        public ActionResult Delete(int xuhao)
        {            
            danwei danwei = db.danweis.Where(s => s.xuhao == xuhao).FirstOrDefault();

            if (danwei == null)
            {
                return HttpNotFound();
            }
            return View(danwei);
        }


        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "管理员账号")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int xuhao)
        {

            //xiangmuzongbiao xiangmuzongbiao = await db.xiangmuzongbiaos.FindAsync(id);
            danwei danwei = db.danweis.Where(s => s.xuhao == xuhao).FirstOrDefault();
            db.danweis.Remove(danwei);
            db.SaveChanges();
            return RedirectToAction("Index", "danwei");

        }
    }
}