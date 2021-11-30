using PagedList;
using PhongKhamNhi.Models.DTO;
using PhongKhamNhi.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PhongKhamNhi.Models.DAO
{
    public class BacSiDAO
    {
        ModelPkNhi db;
        public BacSiDAO()
        {
            db = new ModelPkNhi();
        }

        public IEnumerable<BacSi> GetListBs(int pageNum, int pageSize)
        {
            var lst = db.Database.SqlQuery<BacSi>("SELECT * FROM BacSi").ToPagedList<BacSi>(pageNum, pageSize);
            return lst;
        }
        public BacSi GetBacSi(int idTk)
        {
            var res = (from s in db.BacSis where s.IdTaiKhoan == idTk select s);
            if (res.Count() > 0)
                return res.FirstOrDefault();
            return null;
        }
        public List<BacSi> GetListBacSiByMaCn(int maCn)
        {
            var res = (from s in db.BacSis where s.MaChiNhanh == maCn select s);
            foreach(BacSi item in res)
            {
                item.PhieuDangKyKhams.Clear();
                item.PhieuDangKyKhams = (from s in db.PhieuDangKyKhams where s.TrangThai == true && s.MaBS == item.MaBS orderby s.ThoiGianHen select s).ToList();
            }
            return res.ToList();
        }
        public List<BacSiDTO> GetListTgKhamBs(int maCn)
        {
            List<BacSiDTO> lst = new List<BacSiDTO>();
            var res = db.Database.SqlQuery<BacSi>(string.Format("SELECT * FROM BacSi WHERE MaChiNhanh = {0}", maCn));
            foreach (BacSi item in res)
            {
                BacSiDTO bs = new BacSiDTO();
                bs.MaBS = item.MaBS;
                bs.HoTen = item.HoTen;
                bs.sldk = (from s in db.PhieuKhamBenhs where s.TrangThai == 1 && s.MaBS == item.MaBS && s.ThoiGianKham == null select s).ToList().Count;
                bs.sldk += (from s in db.PhieuDangKyKhams where s.ThoiGianHen > DateTime.Now && s.TrangThai == true && s.MaBS == item.MaBS select s).ToList().Count;
                lst.Add(bs);
            }
            return lst;
        }
        public BacSi FindByID(int id)
        {
            return db.BacSis.Find(id);
        }
    }
}