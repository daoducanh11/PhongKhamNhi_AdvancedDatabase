using PhongKhamNhi.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PhongKhamNhi.Models.DAO
{
    public class DoanhThuDAO
    {
        ModelPkNhi db;
        public DoanhThuDAO()
        {
            db = new ModelPkNhi();
        }

        public DoanhThu Find(DateTime d, int cn)
        {
            var res = db.Database.SqlQuery<DoanhThu>(string.Format("SELECT * FROM DoanhThu WHERE MaChiNhanh = {0} AND NgayThangNam = '{1}'",
                cn, d.ToString("yyyy-MM-dd")));
            if (res != null)
                return res.FirstOrDefault();
            return null;
        }
        public int Insert(DoanhThu d)
        {
            db.DoanhThus.Add(d);//luu tren RAM
            db.SaveChanges();//luu vao o dia
            return 1;
        }
        public int Update(DoanhThu d)
        {
            DoanhThu tmp = db.DoanhThus.Find(d.NgayThangNam, d.MaChiNhanh);
            if (tmp != null)
            {
                tmp.ThuDichVuKham = d.ThuDichVuKham;
                tmp.ThuBanThuoc = d.ThuBanThuoc;
                tmp.ThuXetNghiem = d.ThuXetNghiem;
                tmp.TongTien = d.TongTien;
                db.SaveChanges();//luu vao o dia
            }
            return 1;
        }
    }
}