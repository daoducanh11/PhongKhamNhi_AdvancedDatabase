﻿using PagedList;
using PhongKhamNhi.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PhongKhamNhi.Models.DAO
{
    public class PhieuDangKyKhamDAO
    {
        ModelPkNhi db;
        public PhieuDangKyKhamDAO()
        {
            db = new ModelPkNhi();
        }
        public IEnumerable<PhieuDangKyKham> lstDk(int cn, string sdt, string tgDk, string tgHen, string trangThai, int pageNum, int pageSize)
        {
            var lst = db.Database.SqlQuery<PhieuDangKyKham>(string.Format("lstPhieuDkk {0}, '{1}', '{2}', '{3}', '{4}'",
                cn, sdt, tgDk, tgHen, trangThai)
                ).ToPagedList<PhieuDangKyKham>(pageNum, pageSize);
            return lst;
        }
        public List<PhieuDangKyKham> FindDkkByMaBs(int cn, int bs)
        {
            var res = (from s in db.PhieuDangKyKhams where s.MaChiNhanh == cn && s.MaBS == bs && s.TrangThai == true orderby s.ThoiGianHen select s);
            return res.ToList();
        }
        public int SlPhieuDk(int cn)
        {
            var res = (from s in db.PhieuDangKyKhams where s.MaChiNhanh == cn && s.TrangThai == false select s);
            return res.Count();
        }


        public PhieuDangKyKham FindByID(int id)
        {
            return db.PhieuDangKyKhams.Find(id);
        }
        public int Insert(PhieuDangKyKham p)
        {
            db.PhieuDangKyKhams.Add(p);//luu tren RAM
            db.SaveChanges();//luu vao o dia
            return p.MaPhieuDKK;
        }
        public int Update(PhieuDangKyKham p)
        {
            PhieuDangKyKham dk = db.PhieuDangKyKhams.Find(p.MaPhieuDKK);
            if (dk != null)
            {
                dk.MaBS = p.MaBS;
                dk.HoTen = p.HoTen;
                dk.LoiNhan = p.LoiNhan;
                dk.MaChiNhanh = p.MaChiNhanh;
                dk.MaNV = p.MaNV;
                dk.NgaySinh = p.NgaySinh;
                dk.Sdt = p.Sdt;
                dk.ThoiGianDKK = p.ThoiGianDKK;
                dk.ThoiGianHen = p.ThoiGianHen;
                dk.TrangThai = p.TrangThai;
                db.SaveChanges();//luu vao o dia
            }
            return dk.MaPhieuDKK;
        }
        public int Delete(int id)
        {
            PhieuDangKyKham dkk = db.PhieuDangKyKhams.Find(id);
            if (dkk != null)
            {
                db.PhieuDangKyKhams.Remove(dkk);
                return db.SaveChanges();
            }
            else
                return -1;
        }

        public int UpdateBySERIALIZABLE(PhieuDangKyKham p)
        {
            db.Database.ExecuteSqlCommand(string.Format("CapNhatPhieuDKK {0}, {1}, {2}, {3}, '{4}', N'{5}', " +
                "'{6}', '{7}', '{8}', N'{9}', '{10}'", p.MaPhieuDKK, p.MaChiNhanh, p.MaNV, p.MaBS, 
                p.ThoiGianDKK.Value.ToString("yyyy-MM-dd HH:mm:ss"), p.HoTen, p.NgaySinh.ToString("yyyy-MM-dd"), 
                p.Sdt, p.ThoiGianHen.ToString("yyyy-MM-dd HH:mm:ss"), p.LoiNhan, p.TrangThai));
            return p.MaPhieuDKK;
        }
    }
}