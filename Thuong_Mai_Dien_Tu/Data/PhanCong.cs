using System;
using System.Collections.Generic;

namespace Thuong_Mai_Dien_Tu.Data;

public partial class PhanCong
{
    public int MaPc { get; set; }

    public string MaNv { get; set; } = null!;

    public string MaPb { get; set; } = null!;

    public DateTime? NgayPc { get; set; }

    public bool? HieuLuc { get; set; }

    public virtual NhanVien MaNvNavigation { get; set; } = null!;

    public virtual PhongBan MaPbNavigation { get; set; } = null!;
}
