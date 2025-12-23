using System.Data;
using System.Text.Json;

namespace ErtushevShop.Models
{
    public class CartService
    {
        private readonly ISession _session;

        public CartService(IHttpContextAccessor httpContextAccessor)
        {
            _session = httpContextAccessor.HttpContext.Session;
        }

        public List<CartItem> GetCart()
        {
            var cartJson = _session.GetString("Cart");
            return cartJson == null
                ? new List<CartItem>()
                : JsonSerializer.Deserialize<List<CartItem>>(cartJson);
        }

        public void AddToCart(int productId)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.ProductId == productId);

            if (item != null)
                item.Quantity++;
            else
                cart.Add(new CartItem { ProductId = productId, Quantity = 1 });

            _session.SetString("Cart", JsonSerializer.Serialize(cart));
        }

        public void RemoveFromCart(int productId)
        {
            var cart = GetCart();
            cart.RemoveAll(c => c.ProductId == productId);
            _session.SetString("Cart", JsonSerializer.Serialize(cart));
        }

        public void UpdateQuantity(int productId, int quantity)
        {
            if (quantity < 1) return;

            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.ProductId == productId);
            if (item != null)
                item.Quantity = quantity;

            _session.SetString("Cart", JsonSerializer.Serialize(cart));
        }

        public void ClearCart()
        {
            var cart = GetCart();
            cart.Clear();
            _session.SetString("Cart", JsonSerializer.Serialize(cart));
        }
    }

    public class CartItem
    {
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }
}
