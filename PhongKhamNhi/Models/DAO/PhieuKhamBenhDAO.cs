﻿using PagedList;
using PhongKhamNhi.Models.DTO;
using PhongKhamNhi.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PhongKhamNhi.Models.DAO
{
    public class PhieuKhamBenhDAO
    {
        ModelPkNhi db;
        public PhieuKhamBenhDAO()
        {
            db = new ModelPkNhi();
        }

        public IEnumerable<PhieuKhamDTO> lstPhieuKb(int cn, string ten, int dv, int bs, int trangThai, int pageNum, int pageSize)
        {
            var lst = db.Database.SqlQuery<PhieuKhamDTO>(string.Format("lstPhieuKb {0}, N'{1}', {2}, {3}, {4}",
                cn, ten, dv, bs, trangThai)
                ).ToPagedList<PhieuKhamDTO>(pageNum, pageSize);
            return lst;
        }
        public IEnumerable<PhieuKhamDTO> lstPhieuKb2(int cn, string ten, int dv, int bs, int trangThai, string tu, string den, int pageNum, int pageSize)
        {
            var lst = db.Database.SqlQuery<PhieuKhamDTO>(string.Format("lstPhieuKb2 {0}, N'{1}', {2}, {3}, {4}, '{5}', '{6}'",
                cn, ten, dv, bs, trangThai, tu, den)
                ).ToPagedList<PhieuKhamDTO>(pageNum, pageSize);
            return lst;
        }
        public IEnumerable<PhieuKhamBenh> lichSuKham(int id, int pageNum, int pageSize)
        {
            var lst = db.Database.SqlQuery<PhieuKhamBenh>(string.Format("SELECT * FROM PhieuKhamBenh WHERE MaBN = {0} ORDER BY MaPhieuKB DESC",
                id)
                ).ToPagedList<PhieuKhamBenh>(pageNum, pageSize);
            return lst;
        }

        public int SlPhieuKb(int cn)
        {
            var res = (from s in db.PhieuKhamBenhs where s.MaChiNhanh == cn && s.TrangThai == 0 select s);
            return res.Count();
        }
        public int SlPhieuDkXn(int cn)
        {
            var res = (from s in db.PhieuDKXNs where s.PhieuKhamBenh.MaChiNhanh == cn && s.TrangThai == 0 select s);
            return res.Count();
        }
        public int SlHoaDon(int cn)
        {
            var res = (from s in db.HoaDonBanThuocs where s.MaChiNhanh == cn && s.TrangThai == false select s);
            return res.Count();
        }
        public int SlPhieuKbOfBs(int cn, int bs)
        {
            var res = (from s in db.PhieuKhamBenhs where s.MaBS == bs && s.MaChiNhanh == cn && s.TrangThai == 1 select s);
            return res.Count();
        }

        public PhieuKhamBenh FindByID(int id)
        {
            return db.PhieuKhamBenhs.Find(id);
        }
        public int Insert(PhieuKhamBenh p)
        {
            db.PhieuKhamBenhs.Add(p);//luu tren RAM
            db.SaveChanges();//luu vao o dia
            return p.MaPhieuKB;
        }
        public int UpdateLeTan(PhieuKhamBenh p)
        {
            PhieuKhamBenh tmp = db.PhieuKhamBenhs.Find(p.MaPhieuKB);
            if (tmp != null)
            {
                tmp.DonGia = p.DonGia;
                tmp.MaBS = p.MaBS;
                tmp.MaDV = p.MaDV;
                tmp.MaNvLap = p.MaNvLap;
                tmp.ThoiGianLap = p.ThoiGianLap;
                db.SaveChanges();//luu vao o dia
            }
            return tmp.MaPhieuKB;
        }
        public int UpdateThuNgan(PhieuKhamBenh p)
        {
            PhieuKhamBenh tmp = db.PhieuKhamBenhs.Find(p.MaPhieuKB);
            if (tmp != null)
            {
                tmp.MaNvThu = p.MaNvThu;
                tmp.TrangThai = p.TrangThai;
                db.SaveChanges();//luu vao o dia
            }
            return tmp.MaPhieuKB;
        }
        public int Update(PhieuKhamBenh p)
        {
            PhieuKhamBenh tmp = db.PhieuKhamBenhs.Find(p.MaPhieuKB);
            if (tmp != null)
            {
                tmp.BieuHien = p.BieuHien;
                tmp.KetLuan = p.KetLuan;
                tmp.TrangThai = 2;
                db.SaveChanges();//luu vao o dia
            }
            return tmp.MaPhieuKB;
        }
        public int Delete(int id)
        {
            PhieuKhamBenh p = db.PhieuKhamBenhs.Find(id);
            if (p != null)
            {
                db.PhieuKhamBenhs.Remove(p);
                return db.SaveChanges();
            }
            else
                return -1;
        }
    }
}