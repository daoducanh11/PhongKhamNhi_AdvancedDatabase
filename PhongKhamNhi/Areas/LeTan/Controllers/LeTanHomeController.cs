using PhongKhamNhi.Models.DAO;
using PhongKhamNhi.Models.DTO;
using PhongKhamNhi.Models.Entities;
using PhongKhamNhi.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PhongKhamNhi.Areas.LeTan.Controllers
{
    //[AuthorizationLogin]
    public class LeTanHomeController : Controller
    {
        // GET: LeTan/LeTanHome
        public ActionResult Index(string sdt, string tgDk, string tgHen, string trangThai, int pageNum = 1, int pageSize = 9)
        {
            ViewBag.sdt = sdt;
            ViewBag.tgDk = tgDk;
            ViewBag.tgHen = tgHen;
            ViewBag.trangThai = trangThai;
            PhieuDangKyKhamDAO dao = new PhieuDangKyKhamDAO();
            NhanVien nv = (NhanVien)Session["user"];
            return View(dao.lstDk(nv.MaChiNhanh, sdt, tgDk, tgHen, trangThai, pageNum, pageSize));
        }
        [HttpPost]
        public ActionResult Index(FormCollection data, int pageNum = 1, int pageSize = 9)
        {
            PhieuDangKyKhamDAO dao = new PhieuDangKyKhamDAO();
            if (data.Count > 0)
            {
                string[] ids = data["checkBoxId"].Split(new char[] { ',' });
                foreach (string id in ids)
                {
                    dao.Delete(int.Parse(id));
                }
            }
            NhanVien nv = (NhanVien)Session["user"];
            return View(dao.lstDk(nv.MaChiNhanh, "", "", "", "", pageNum, pageSize));
        }

        public ActionResult Create()
        {
            List<ChiNhanh> lst = new ChiNhanhDAO().ListChiNhanh();
            ViewBag.ListChiNhanh = lst;
            ViewBag.ListBacSi = new BacSiDAO().GetListBacSiByMaCn(lst[0].MaChiNhanh);
            ViewBag.tgHenMin = DateTime.Now.AddDays(1).ToString("yyyy-MM-ddTHH:mm");
            return View();
        }
        [HttpPost]
        public ActionResult Create(PhieuDangKyKham tmp, string chiNhanh, string bs)
        {
            int maCn = int.Parse(chiNhanh);

            tmp.MaChiNhanh = maCn;
            tmp.MaBS = int.Parse(bs);
            NhanVien nv = (NhanVien)Session["user"];
            tmp.MaNV = nv.MaNV;
            tmp.ThoiGianDKK = DateTime.Now;
            tmp.TrangThai = false;
            PhieuDangKyKhamDAO dao = new PhieuDangKyKhamDAO();
            dao.Insert(tmp);
            return RedirectToAction("Index", "LeTanHome");
        }

        public JsonResult CheckTime(int chiNhanh, int bacSi, DateTime time)
        {
            ResultLichHen res = new ResultLichHen();
            List<int> ls = new List<int>();
            res.bs = bacSi;
            List<PhieuDangKyKham> lst = new PhieuDangKyKhamDAO().FindDkkByMaBs(chiNhanh, bacSi);
            foreach(PhieuDangKyKham item in lst)
            {
                TimeSpan t = item.ThoiGianHen - time;
                if (Math.Abs(t.TotalMinutes) <= 30)
                    ls.Add(item.MaPhieuDKK);
            }
            res.pdk = ls;
            return Json(new
            {
                data = res,
                status = true
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadLichHen(int cn, DateTime tgHen)
        {
            List<BacSi> lst = new BacSiDAO().GetListBacSiByMaCn(cn);
            
            ResultLichHen res = new ResultLichHen();
            List<int> ls = new List<int>();
            res.bs = lst[0].MaBS;
            List<PhieuDangKyKham> lstLh = new PhieuDangKyKhamDAO().FindDkkByMaBs(cn, res.bs);
            foreach (PhieuDangKyKham item in lstLh)
            {
                TimeSpan t = item.ThoiGianHen - tgHen;
                if (Math.Abs(t.TotalMinutes) <= 30)
                    ls.Add(item.MaPhieuDKK);
            }
            res.pdk = ls;
            ViewBag.ListLh = res;
            return View(lst);
        }

        public ActionResult LoadBacSi(int cn)
        {
            List<BacSi> lst = new BacSiDAO().GetListBacSiByMaCn(cn);
            return View(lst);
        }


        public ActionResult Edit(int id)
        {
            PhieuDangKyKham p = new PhieuDangKyKhamDAO().FindByID(id);
            ViewBag.ns = p.NgaySinh.ToString("yyyy-MM-dd");
            ViewBag.tgHen = p.ThoiGianHen.ToString("yyyy-MM-ddTHH:mm");
            List<ChiNhanh> lst = new ChiNhanhDAO().ListChiNhanh();
            ViewBag.ListChiNhanh = lst;
            ViewBag.ListBacSi = new BacSiDAO().GetListBacSiByMaCn(lst[0].MaChiNhanh);
            ViewBag.tgHenMin = DateTime.Now.AddDays(1).ToString("yyyy-MM-ddTHH:mm");
            return View(p);
        }
        [HttpPost]
        public ActionResult Edit(PhieuDangKyKham p, DateTime NgaySinh, DateTime ThoiGianHen, string tt, string chiNhanh, string bs)
        {
            p.NgaySinh = NgaySinh;
            p.ThoiGianHen = ThoiGianHen;
            if (tt == "0")
                p.TrangThai = false;
            else
                p.TrangThai = true;
            p.MaChiNhanh = int.Parse(chiNhanh);
            p.MaBS = int.Parse(bs);
            NhanVien nv = (NhanVien)Session["user"];
            p.MaNV = nv.MaNV;
            new PhieuDangKyKhamDAO().UpdateBySERIALIZABLE(p);
            return RedirectToAction("Index", "LeTanHome");

            //if (ModelState.IsValid)
            //{
            //    p.NgaySinh = NgaySinh;
            //    p.ThoiGianHen = ThoiGianHen;
            //    if (tt == "0")
            //        p.TrangThai = false;
            //    else
            //        p.TrangThai = true;
            //    p.MaChiNhanh = int.Parse(chiNhanh);
            //    p.MaBS = int.Parse(bs);
            //    NhanVien nv = (NhanVien)Session["user"];
            //    p.MaNV = nv.MaNV;
            //    new PhieuDangKyKhamDAO().Update(p);
            //    return RedirectToAction("Index", "LeTanHome");
            //}
            //ViewBag.ns = NgaySinh.ToString("yyyy-MM-dd");
            //ViewBag.tgHen = ThoiGianHen.ToString("yyyy-MM-ddTHH:mm");
            //List<ChiNhanh> lst = new ChiNhanhDAO().ListChiNhanh();
            //ViewBag.ListChiNhanh = lst;
            //ViewBag.ListBacSi = new BacSiDAO().GetListBacSiByMaCn(int.Parse(chiNhanh));
            //ViewBag.tgHenMin = DateTime.Now.AddDays(1).ToString("yyyy-MM-ddTHH:mm");
            //return View(p);
        }

        public ActionResult Delete(int id)
        {
            new PhieuDangKyKhamDAO().Delete(id);
            return RedirectToAction("Index", "LeTanHome");
        }

        public ActionResult SlPhieuDk()
        {
            NhanVien nv = (NhanVien)Session["user"];
            return PartialView(new PhieuDangKyKhamDAO().SlPhieuDk(nv.MaChiNhanh));
        }
    }
    public class ResultLichHen
    {
        public int bs { get; set; }
        public List<int> pdk { get; set; }
    }

    
}