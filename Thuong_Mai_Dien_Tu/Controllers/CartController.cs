using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Thuong_Mai_Dien_Tu.Data;
using Thuong_Mai_Dien_Tu.Helpers;
using Thuong_Mai_Dien_Tu.ViewModels;

namespace Thuong_Mai_Dien_Tu.Controllers
{
    public class CartController : Controller
    {
        private readonly Hshop2023Context db;

        public CartController(Hshop2023Context context)
        {
            db = context;
        }

        // Sử dụng đúng tên class CartiItem như bạn yêu cầu
        public List<CartiItem> Cart => HttpContext.Session.Get<List<CartiItem>>(MySetting.CART_KEY) ?? new List<CartiItem>();

        public IActionResult Index()
        {
            return View(Cart);
        }

        // File: Controllers/CartController.cs

        public IActionResult AddToCart(int id, int quantity = 1)
        {
            var gioHang = Cart;
            var item = gioHang.SingleOrDefault(p => p.MaHh == id);

            if (item == null)
            {
                var hangHoa = db.HangHoas.SingleOrDefault(p => p.MaHh == id);
                if (hangHoa == null)
                {
                    TempData["Message"] = $"Không tìm thấy hàng hóa có mã {id}";
                    return Redirect("/404");
                }
                item = new CartiItem
                {
                    MaHh = hangHoa.MaHh,
                    TenHH = hangHoa.TenHh,
                    DonGia = hangHoa.DonGia ?? 0,
                    Hinh = hangHoa.Hinh ?? string.Empty,
                    SoLuong = quantity
                };
                gioHang.Add(item);
            }
            else
            {
                item.SoLuong += quantity;
            }

            // MỚI: Nếu số lượng <= 0 thì xóa sản phẩm khỏi giỏ
            if (item.SoLuong <= 0)
            {
                gioHang.Remove(item);
            }

            HttpContext.Session.Set(MySetting.CART_KEY, gioHang);
            return RedirectToAction("Index");
        }

        public IActionResult RemoveCart(int id)
        {
            var gioHang = Cart;
            var item = gioHang.SingleOrDefault(p => p.MaHh == id);
            if (item != null)
            {
                gioHang.Remove(item);
                HttpContext.Session.Set(MySetting.CART_KEY, gioHang);
            }
            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpGet]
        public IActionResult Checkout()
        {
            if (Cart.Count == 0)
            {
                return Redirect("/");
            }
            return View(Cart);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Checkout(CheckoutVM model)
        {
            if (ModelState.IsValid)
            {
                var customerId = HttpContext.User.Claims.SingleOrDefault(p => p.Type == MySetting.CLAIM_CUSTOMERID).Value;
                var khachHang = new KhachHang();
                if (model.GiongKhachHang)
                {
                    khachHang = db.KhachHangs.SingleOrDefault(kh => kh.MaKh == customerId);
                }

                var hoadon = new HoaDon
                {
                    MaKh = customerId,
                    HoTen = model.HoTen ?? khachHang?.HoTen,
                    DiaChi = model.DiaChi ?? khachHang?.DiaChi,
                    DienThoai = model.DienThoai ?? khachHang?.DienThoai, // Giữ nguyên nếu bạn đã thêm cột DienThoai vào SQL
                                                                         // Nếu chưa thêm cột DienThoai vào SQL, hãy dùng dòng dưới đây thay thế dòng trên:
                                                                         // GhiChu = model.GhiChu + (string.IsNullOrEmpty(model.DienThoai) ? "" : $"; SĐT: {model.DienThoai}"), 

                    NgayDat = DateTime.Now,
                    CachThanhToan = "COD",
                    CachVanChuyen = "GRAB",
                    MaTrangThai = 0,
                    GhiChu = model.GhiChu,
                    NgayCan = DateTime.Now.AddDays(3)
                };

                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        db.Add(hoadon);
                        db.SaveChanges();

                        var cthds = new List<ChiTietHd>();
                        foreach (var item in Cart)
                        {
                            cthds.Add(new ChiTietHd
                            {
                                MaHd = hoadon.MaHd,
                                SoLuong = item.SoLuong,
                                DonGia = item.DonGia,
                                MaHh = item.MaHh,
                                GiamGia = 0
                            });
                        }
                        db.AddRange(cthds);
                        db.SaveChanges();

                        transaction.Commit();

                        // Xóa giỏ hàng với đúng kiểu dữ liệu CartiItem
                        HttpContext.Session.Set<List<CartiItem>>(MySetting.CART_KEY, new List<CartiItem>());

                        return View("Success");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        ModelState.AddModelError("", "Lỗi xử lý đơn hàng: " + ex.Message);
                    }
                }
            }
            return View(Cart);
        }

        public IActionResult Success()
        {
            return View();
        }
    }
}