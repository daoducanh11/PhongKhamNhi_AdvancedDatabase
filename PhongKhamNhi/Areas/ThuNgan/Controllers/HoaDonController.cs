﻿using PhongKhamNhi.Models.DAO;
using PhongKhamNhi.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PhongKhamNhi.Areas.ThuNgan.Controllers
{
    public class HoaDonController : Controller
    {
        // GET: ThuNgan/HoaDon
        public ActionResult Index(string maHd, string ten, string tu, string den, int pageNum = 1, int pageSize = 9)
        {
            ViewBag.ten = ten;
            ViewBag.tu = tu;
            ViewBag.den = den;
            if (maHd == null)
                maHd = "0";
            if (tu == null)
                tu = "2020-11-19 12:00:00";
            if (den == null)
                den = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            NhanVien nv = (NhanVien)Session["user"];
            return View(new HoaDonThuocDAO().ListHdThuoc(nv.MaChiNhanh, int.Parse(maHd), ten, tu, den, pageNum, pageSize));
        }
        public JsonResult ChangeStatus(int id)
        {
            HoaDonThuocDAO dao = new HoaDonThuocDAO();
            HoaDonBanThuoc p = dao.FindByID(id);
            NhanVien nv = (NhanVien)Session["user"];
            p.MaNvLap = nv.MaNV;
            DoanhThuDAO daodt = new DoanhThuDAO();
            DoanhThu d = daodt.Find(p.ThoiGian, p.MaChiNhanh);
            if (p.TrangThai == false)
            {
                p.TrangThai = true;
                if (d != null)
                {
                    d.ThuBanThuoc += p.TongTien;
                    d.TongTien += p.TongTien;
                    daodt.Update(d);
                }
                else
                {
                    d = new DoanhThu();
                    d.NgayThangNam = p.ThoiGian;
                    d.MaChiNhanh = p.MaChiNhanh;
                    d.ThuDichVuKham = 0;
                    d.ThuXetNghiem = 0;
                    d.ThuBanThuoc = p.TongTien;
                    d.TongTien = p.TongTien;
                    daodt.Insert(d);
                }
            }
            else if (p.TrangThai == true)
            {
                p.TrangThai = false;
                if (d != null)
                {
                    d.ThuBanThuoc -= p.TongTien;
                    d.TongTien -= p.TongTien;
                    daodt.Update(d);
                }
            }
            dao.UpdateThuNgan(p);
            return Json(new
            {
                status = true
            }, JsonRequestBehavior.AllowGet);
        }
    }
}