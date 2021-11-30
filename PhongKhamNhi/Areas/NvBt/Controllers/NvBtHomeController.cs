﻿using PhongKhamNhi.Models.DAO;
using PhongKhamNhi.Models.DTO;
using PhongKhamNhi.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PhongKhamNhi.Areas.NvBt.Controllers
{
    public class NvBtHomeController : Controller
    {
        // GET: NvBt/NvBtHome
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
        [HttpPost]
        public ActionResult Index(FormCollection data, int pageNum = 1, int pageSize = 9)
        {
            HoaDonThuocDAO dao = new HoaDonThuocDAO();
            if (data.Count > 0)
            {
                string[] ids = data["checkBoxId"].Split(new char[] { ',' });
                foreach (string id in ids)
                {
                    dao.Delete(int.Parse(id));
                }
            }
            string tu = "2020-11-19 12:00:00";
            string den = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            NhanVien nv = (NhanVien)Session["user"];
            return View(new HoaDonThuocDAO().ListHdThuoc(nv.MaChiNhanh, 0, "", tu, den, pageNum, pageSize));
        }

        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(HoaDonBanThuoc h)
        {
            h.ThoiGian = DateTime.Now;
            h.TrangThai = false;
            NhanVien nv = (NhanVien)Session["user"];
            h.MaChiNhanh = nv.MaChiNhanh;
            h.MaNvLap = nv.MaNV;
            HoaDonThuocDAO dao = new HoaDonThuocDAO();
            int id = dao.Insert(h);
            List<CtHdThuocDTO> lst = (List<CtHdThuocDTO>)Session["cart"];
            double t = 0;
            foreach (CtHdThuocDTO i in lst)
            {
                dao.InsertCt(i.MaThuoc, id, i.SoLuong, i.DonGia);
                t += i.DonGia * i.SoLuong;
            }
            dao.UpdatetongTien(id, t);
            //DoanhThuDAO daoDt = new DoanhThuDAO();
            //DoanhThu d = daoDt.Find(h.ThoiGian, h.MaChiNhanh);
            //d.ThuBanThuoc += t;
            //d.TongTien += t;
            //daoDt.Update(d);
            return RedirectToAction("Index", "NvBtHome");
        }

        public ActionResult Edit(int id)
        {
            HoaDonThuocDAO dao = new HoaDonThuocDAO();
            HoaDonBanThuoc h = dao.FindByID(id);
            List<CtHdThuocDTO> lst = dao.lstThuocByMaHd(id);
            double t = 0;
            foreach (CtHdThuocDTO i in lst)
            {
                t += i.SoLuong * i.DonGia;
            }
            ViewBag.tong = t;
            ViewBag.ListThuoc = lst;
            Session["cart"] = lst;
            return View(h);
        }
        [HttpPost]
        public ActionResult Edit(HoaDonBanThuoc h)
        {
            DateTime dt = h.ThoiGian;
            h.ThoiGian = DateTime.Now;
            NhanVien nv = (NhanVien)Session["user"];
            h.MaChiNhanh = nv.MaChiNhanh;
            h.MaNvLap = nv.MaNV;
            HoaDonThuocDAO dao = new HoaDonThuocDAO();
            int id = dao.Update(h);
            dao.DeleteThuoc(id);
            List<CtHdThuocDTO> lst = (List<CtHdThuocDTO>)Session["cart"];
            double t = 0;
            foreach (CtHdThuocDTO i in lst)
            {
                dao.InsertCt(i.MaThuoc, id, i.SoLuong, i.DonGia);
                t += i.DonGia * i.SoLuong;
            }
            dao.UpdatetongTien(id, t);
            //DoanhThuDAO daoDt = new DoanhThuDAO();
            //DoanhThu d0 = daoDt.Find(dt, h.MaChiNhanh);
            //d0.ThuBanThuoc -= t;
            //d0.TongTien -= t;
            //daoDt.Update(d0);
            //DoanhThu d = daoDt.Find(h.ThoiGian, h.MaChiNhanh);
            //d.ThuBanThuoc += t;
            //d.TongTien += t;
            //daoDt.Update(d);
            return RedirectToAction("Index", "NvBtHome");
        }

        public JsonResult ListThuoc(string key)
        {
            var data = new ThuocDAO().lstSearchThuoc(key);
            return Json(new
            {
                data = data,
                status = true
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult AddThuoc(int id)
        {
            List<CtHdThuocDTO> lst = (List<CtHdThuocDTO>)Session["cart"];
            if (lst == null)
                lst = new List<CtHdThuocDTO>();
            Thuoc item = new ThuocDAO().FindByID(id);
            CtHdThuocDTO c = new CtHdThuocDTO();
            c.MaThuoc = item.MaThuoc;
            c.TenThuoc = item.TenThuoc;
            c.DonViTinh = item.DonViTinh;
            c.DonGia = item.DonGia;
            c.SoLuong = 1;
            lst.Add(c);
            double t = 0;
            foreach (CtHdThuocDTO i in lst)
            {
                t += i.DonGia * i.SoLuong;
            }
            Session["cart"] = lst;
            return Json(new
            {
                data = new ResultSum
                {
                    id = id,
                    value = c.DonGia,
                    totalMoney = t
                },
                status = true
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteThuoc(int id)
        {
            List<CtHdThuocDTO> lst = (List<CtHdThuocDTO>)Session["cart"];
            foreach (CtHdThuocDTO i in lst)
            {
                if (i.MaThuoc == id)
                {
                    lst.Remove(i);
                    break;
                }
            }
            double t = 0;
            foreach (CtHdThuocDTO i in lst)
            {
                t += i.DonGia * i.SoLuong;
            }
            Session["cart"] = lst;
            return Json(new
            {
                data = new ResultSum
                {
                    totalMoney = t
                },
                status = true
            }, JsonRequestBehavior.AllowGet); ;
        }
        public JsonResult UpdateThuoc(int id, int quantity)
        {
            List<CtHdThuocDTO> lst = (List<CtHdThuocDTO>)Session["cart"];
            double s = 0;
            foreach (CtHdThuocDTO i in lst)
            {
                if (i.MaThuoc == id)
                {
                    i.SoLuong = quantity;
                    s = i.SoLuong * i.DonGia;
                    break;
                }
            }
            double t = 0;
            foreach (CtHdThuocDTO i in lst)
            {
                t += i.DonGia * i.SoLuong;
            }
            Session["cart"] = lst;
            return Json(new
            {
                data = new ResultSum
                {
                    id = id,
                    value = s,
                    totalMoney = t
                },
                status = true
            }, JsonRequestBehavior.AllowGet);
        }

    }
    public class ResultSum
    {
        public int id { get; set; }
        public double value { get; set; }
        public double totalMoney { get; set; }
    }
}