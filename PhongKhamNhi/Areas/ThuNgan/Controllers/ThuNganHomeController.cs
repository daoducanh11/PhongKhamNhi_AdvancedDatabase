﻿using PhongKhamNhi.Models.DAO;
using PhongKhamNhi.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PhongKhamNhi.Areas.ThuNgan.Controllers
{
    public class ThuNganHomeController : Controller
    {
        // GET: ThuNgan/ThuNganHome
        public ActionResult Index(string ten, string dv, string bs, string trangThai, int pageNum = 1, int pageSize = 9)
        {
            if (dv == null)
                dv = "0";
            if (bs == null)
                bs = "0";
            if (trangThai == null)
                trangThai = "3";
            ViewBag.ten = ten;
            ViewBag.dv = int.Parse(dv);
            ViewBag.bs = int.Parse(bs);
            ViewBag.trangThai = int.Parse(trangThai);
            PhieuKhamBenhDAO dao = new PhieuKhamBenhDAO();
            NhanVien nv = (NhanVien)Session["user"];
            ViewBag.ListBacSi = new BacSiDAO().GetListBacSiByMaCn(nv.MaChiNhanh);
            ViewBag.ListDichVu = new DichVuDAO().GetListDv();
            return View(dao.lstPhieuKb(nv.MaChiNhanh, ten, int.Parse(dv), int.Parse(bs), int.Parse(trangThai), pageNum, pageSize));
        }
        public JsonResult ChangeStatus(int id)
        {
            PhieuKhamBenhDAO dao = new PhieuKhamBenhDAO();
            PhieuKhamBenh p = dao.FindByID(id);
            NhanVien nv = (NhanVien)Session["user"];
            p.MaNvLap = nv.MaNV;
            DoanhThuDAO daodt = new DoanhThuDAO();
            DoanhThu d = daodt.Find(p.ThoiGianLap, p.MaChiNhanh);
            if (p.TrangThai == 0)
            {
                p.TrangThai = 1;
                if (d != null)
                {
                    d.ThuDichVuKham += p.DonGia;
                    d.TongTien += p.DonGia;
                    daodt.Update(d);
                }
                else
                {
                    d = new DoanhThu();
                    d.NgayThangNam = p.ThoiGianLap;
                    d.MaChiNhanh = p.MaChiNhanh;
                    d.ThuDichVuKham = p.DonGia;
                    d.ThuXetNghiem = 0;
                    d.ThuBanThuoc = 0;
                    d.TongTien = p.DonGia;
                    daodt.Insert(d);
                }
            }    
            else if (p.TrangThai == 1)
            {
                p.TrangThai = 0;
                if (d != null)
                {
                    d.ThuDichVuKham -= p.DonGia;
                    d.TongTien -= p.DonGia;
                    daodt.Update(d);
                }
            }    
                
            dao.UpdateThuNgan(p);
            return Json(new
            {
                status = true
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SlPhieuKb()
        {
            NhanVien nv = (NhanVien)Session["user"];
            return PartialView(new PhieuKhamBenhDAO().SlPhieuKb(nv.MaChiNhanh));
        }
        public ActionResult SlPhieuDkXn()
        {
            NhanVien nv = (NhanVien)Session["user"];
            return PartialView(new PhieuKhamBenhDAO().SlPhieuDkXn(nv.MaChiNhanh));
        }
        public ActionResult SlHoaDon()
        {
            NhanVien nv = (NhanVien)Session["user"];
            return PartialView(new PhieuKhamBenhDAO().SlHoaDon(nv.MaChiNhanh));
        }
    }
}