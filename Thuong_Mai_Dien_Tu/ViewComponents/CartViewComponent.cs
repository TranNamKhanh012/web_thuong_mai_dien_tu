using Microsoft.AspNetCore.Mvc;
using Thuong_Mai_Dien_Tu.Helpers;
using Thuong_Mai_Dien_Tu.ViewModels;

namespace Thuong_Mai_Dien_Tu.ViewComponents
{
    public class CartViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var cart = HttpContext.Session.Get<List<CartiItem>>
                (MySetting.CART_KEY) ?? new List<CartiItem>();
                return View("CartPanel", new CartModel
                {
                    Quantity = cart.Sum(p=>p.SoLuong),
                    Total = cart.Sum (p=>p.ThanhTien)
                });
        }
    }
}
