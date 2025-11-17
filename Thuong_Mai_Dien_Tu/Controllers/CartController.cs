
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
        public string CART_KEY = "MYCART";
        public List<CartiItem> Cart => HttpContext.Session.Get<List<CartiItem>>
            (MySetting.CART_KEY) ?? new List<CartiItem>();
        public IActionResult Index()
        {
            return View(Cart);
        }
        public IActionResult AddToCart(int id , int quantity = 1)
        {
            var gioHang = Cart;
            var item = gioHang.SingleOrDefault(p=>p.MaHh==id);
            if ((item == null))
            {
                var hangHoa = db.HangHoas.SingleOrDefault(p=>p.MaHh == id);
                if(hangHoa == null)
                {
                    TempData["Message"] = $"Khong tim thay hang hoa co ma {id}";
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
            HttpContext.Session.Set(MySetting.CART_KEY, gioHang);
            return RedirectToAction("Index");

        }
        public IActionResult RemoveCart(int id)
        {
            var gioHang = Cart;
            var item = gioHang.SingleOrDefault(p=>p.MaHh==id);
            if (item != null)
            {
                gioHang.Remove(item);
                HttpContext.Session.Set(MySetting.CART_KEY , gioHang);
            }
            return RedirectToAction("Index");
        }
        [Authorize]
        public IActionResult Checkout()
        {
            if(Cart.Count == 0)
            {
                return Redirect("/");
            }
            return View(Cart);
        }
    }
}
