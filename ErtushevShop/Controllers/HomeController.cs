using ErtushevShop.Models;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Data;
using System.Diagnostics;
using System.Net;
using System.Numerics;
using System.Text.Json;

namespace ErtushevShop.Controllers
{
    public class HomeController : Controller
    {
        Database database = new Database();

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, CartService cartService)
        {
            _logger = logger;
            _cartService = cartService;
        }

        private readonly CartService _cartService;

        private List<Product> GetProductsFromDB()
        {
            DataTable dtGetData = new DataTable();

            dtGetData = database.GetData("SELECT id, brand, model, category, descript, price FROM cameras UNION ALL SELECT id, brand, model, category, descript, price FROM actioncameras UNION ALL SELECT id, brand, model, category, descript, price FROM copters;");

            List<Product> productList = new List<Product>();

            foreach (DataRow row in dtGetData.Rows)
            {
                Product product = new Product(
                        id: Convert.ToInt32(row[0]),
                        brand: row[1].ToString(),
                        model: row[2].ToString(),
                        descript: row[4].ToString(),
                        category: row[3].ToString(),
                        image: $"/img/{row[0]}.jpg",
                        price: Convert.ToInt32(row[5])
                    );
                productList.Add(product);
            }

            return productList;
        }

        public IActionResult Index(string searchTerm = null, string selectedCategory = null, string sortBy = null)
        {
            List<Product> products = GetProductsFromDB();

            ViewBag.Categories = products
                .Select(p => p.Category)
                .Distinct()
                .ToList();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                products = products.Where(p =>
                    p.Brand.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    p.Model.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (!string.IsNullOrEmpty(selectedCategory))
            {
                products = products.Where(p => p.Category == selectedCategory).ToList();
            }

            products = ApplySorting(products, sortBy);

            return View(products);
        }

        private List<Product> ApplySorting(List<Product> products, string sortOption)
        {
            return sortOption switch
            {
                "price_asc" => products.OrderBy(p => p.Price).ToList(),
                "price_desc" => products.OrderByDescending(p => p.Price).ToList(),
                "name_asc" => products.OrderBy(p => $"{p.Brand} {p.Model}").ToList(),
                "name_desc" => products.OrderByDescending(p => $"{p.Brand} {p.Model}").ToList(),
                _ => products
            };
        }

        public IActionResult Details(int id)
        {
            if (id / 100000 == 4)
            {
                DataTable dtGetDataCameras = new DataTable();
                dtGetDataCameras = database.GetData($"select * from cameras where id = {id};");

                foreach (DataRow row in dtGetDataCameras.Rows)
                {
                    Cameras camera = new Cameras(
                    id: Convert.ToInt32(row[0]),
                    brand: row[2].ToString(),
                    model: row[3].ToString(),
                    descript: row[4].ToString(),
                    category: row[1].ToString(),
                    image: $"/img/{row[0]}.jpg",
                    price: Convert.ToInt32(row[5]),
                    sensorsize: row[6].ToString(),
                    shutterspeed: row[7].ToString(),
                    aperture: row[8].ToString(),
                    isorange: row[9].ToString(),
                    photores: row[10].ToString());

                    Product product = camera;
                    return View(product);
                }
            }

            if (id / 100000 == 7)
            {
                DataTable dtGetDataActionCameras = new DataTable();
                dtGetDataActionCameras = database.GetData($"select * from actioncameras where id = {id};");

                foreach (DataRow row in dtGetDataActionCameras.Rows)
                {
                    ActionCameras actioncamera = new ActionCameras(
                    id: Convert.ToInt32(row[0]),
                    brand: row[2].ToString(),
                    model: row[3].ToString(),
                    descript: row[4].ToString(),
                    category: row[1].ToString(),
                    image: $"/img/{row[0]}.jpg",
                    price: Convert.ToInt32(row[5]),
                    viewangle: Convert.ToInt32(row[6]),
                    depth: Convert.ToInt32(row[7]),
                    fixation: row[8].ToString(),
                    appmanage: row[9].ToString(),
                    isorange: row[10].ToString());

                    Product product = actioncamera;
                    return View(product);
                }
            }

            if (id / 100000 == 9)
            {
                DataTable dtGetDataCopters = new DataTable();
                dtGetDataCopters = database.GetData($"select * from copters where id = {id};");

                foreach (DataRow row in dtGetDataCopters.Rows)
                {
                    Copters copter = new Copters(
                    id: Convert.ToInt32(row[0]),
                    brand: row[2].ToString(),
                    model: row[3].ToString(),
                    descript: row[4].ToString(),
                    category: row[1].ToString(),
                    image: $"/img/{row[0]}.jpg",
                    price: Convert.ToInt32(row[5]),
                    speed: Convert.ToInt32(row[6]),
                    height: Convert.ToInt32(row[7]),
                    duration: Convert.ToInt32(row[8]),
                    appmanage: row[9].ToString(),
                    photores: row[10].ToString());

                    Product product = copter;
                    return View(product);
                }
            }
            return View();
        }

        public IActionResult Cart()
        {
            List<Product> productList = GetProductsFromDB();

            var cartItems = _cartService.GetCart();

            var products = GetProductsFromDB();
            foreach (var item in cartItems)
            {
                item.Product = products.FirstOrDefault(p => p.Id == item.ProductId);
            }

            return View(cartItems);
        }

        [HttpPost]
        public IActionResult AddToCart(int id)
        {
            _cartService.AddToCart(id);
            return Ok();
        }

        [HttpPost]
        public IActionResult AddToCartFromIndex(int id)
        {
            _cartService.AddToCart(id);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int id)
        {
            _cartService.RemoveFromCart(id);
            return RedirectToAction("Cart");
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int id, int quantity)
        {
            _cartService.UpdateQuantity(id, quantity);
            return RedirectToAction("Cart");
        }

        public IActionResult Favorite()
        {
            var viewModel = new Dictionary<int, (Product product, int count)>();

            return View(viewModel);
        }

        public IActionResult Profile()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(int totalAmount, string lastName, string firstName, string middleName, string phone, string email, string address)
        {
            if (!ModelState.IsValid)
            {
                return View("Cart", _cartService.GetCart());
            }

            var cartItems = _cartService.GetCart();

            List<Product> products = GetProductsFromDB();

            foreach (var item in cartItems)
            {
                item.Product = products.FirstOrDefault(p => p.Id == item.ProductId);
            }

            if (!cartItems.Any())
            {
                ModelState.AddModelError("", "Корзина пуста");
                return View("Cart", cartItems);
            }
            DateTime today = DateTime.UtcNow.Date;
            
            var order = new Order
            {
                OrderId = Random.Shared.Next(1112,9999),
                FirstName = firstName,
                LastName = lastName,
                MiddleName = middleName,
                Phone = phone,
                Email = email,
                Address = address,
                TotalAmount = totalAmount,
                CreatedAt = today,
                Items = cartItems
            };

            var addQuery = $"insert into orders values ({order.OrderId}, '{order.LastName}', '{order.FirstName}', '{order.MiddleName}', '{order.Phone}', '{order.Email}', '{order.Address}', {order.TotalAmount}, '{order.CreatedAt.ToShortDateString()}')";
            database.GetData(addQuery);

            return RedirectToAction("Order", new {id = order.OrderId});
        }

        public IActionResult Order(int id)
        {
            DataTable dt = new DataTable();
            dt = database.GetData($"select * from orders where id = {id};");

            var row = dt.Rows[0];

            var cartItems = _cartService.GetCart();

            List<Product> products = GetProductsFromDB();

            foreach (var item in cartItems)
            {
                item.Product = products.FirstOrDefault(p => p.Id == item.ProductId);
            }

            Order order = new Order
            {
                OrderId = Convert.ToInt32(row[0]),
                FirstName = row[2].ToString(),
                LastName = row[1].ToString(),
                MiddleName = row[3].ToString(),
                Phone = row[4].ToString(),
                Email = row[5].ToString(),
                Address = row[6].ToString(),
                TotalAmount = Convert.ToInt32(row[7]),
                CreatedAt = DateTime.Parse(row[8].ToString()),
                Items = cartItems
            };

            _cartService.ClearCart();

            return View(order);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
