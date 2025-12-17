using ErtushevShop.Models;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
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

        public bool IsEditing { get; set; }

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

        public IActionResult Index(string searchTerm = null, string selectedCategory = null, string selectedBrand = null, string sortBy = null)
        {
            List<Product> products = GetProductsFromDB();

            ViewBag.Categories = products
                .Select(p => p.Category)
                .Distinct()
                .ToList();

            ViewBag.Brands = products
                .Select(p => p.Brand)
                .Distinct()
                .ToList();

            ViewBag.Categories = products
                .Select(p => p.Category)
                .Distinct()
                .ToList();

            ViewBag.Brands = products
                .Select(p => p.Brand)
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

            if (!string.IsNullOrEmpty(selectedBrand))
            {
                products = products.Where(p => p.Brand == selectedBrand).ToList();
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

        private List<User> GetUsersFromDb()
        {
            DataTable dt = new DataTable();
            dt = database.GetData("select * from users;");
            List<User> users = new List<User>();
            foreach (DataRow row in dt.Rows)
            {
                User user = new User(
                    id: Convert.ToInt32(row[0]),
                    login: row[1].ToString(),
                    password: row[2].ToString(),
                    lastname: row[3].ToString(),
                    firstname: row[4].ToString(),
                    middlename: row[5].ToString(),
                    email: row[6].ToString(),
                    phone: row[7].ToString()
                    );
                users.Add(user);
            }
            return users;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            List<User> users = GetUsersFromDb();

            var user = users.FirstOrDefault(u => u.Login == username && u.Password == password);

            if (user != null)
            {
                HttpContext.Session.SetString("Username", user.Login);
                return Json(new { success = true, redirectUrl = Url.Action("Index", "Home") });
            }
            else
            {
                return Json(new { success = false, message = "Неверное имя или пароль." });
            }
        }

        public ActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult ClearAllCart()
        {
            var cartItems = _cartService.GetCart();
            foreach (var item in cartItems)
            {
                _cartService.RemoveFromCart(item.ProductId);
            }
            return RedirectToAction("Cart", "Home");
        }

        [HttpPost]
        public JsonResult Register(string username, string password)
        {
            List<User> users = GetUsersFromDb();

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return Json(new { success = false, message = "Заполните все поля." });
            }

            if (users.Any(u => u.Login == username))
            {
                return Json(new { success = false, message = "Пользователь с таким именем уже существует." });
            }

            User newUser = new User(
                id: Random.Shared.Next(11111, 99999),
                login: username,
                password: password,
                lastname: null,
                firstname: null,
                middlename: null,
                email: null,
                phone: null
                );


            users.Add(newUser);

            var addQuery = $"insert into users values ({newUser.Id}, '{newUser.Login}', '{newUser.Password}')";
            database.GetData(addQuery);

            return Json(new { success = true, message = "Регистрация успешна! Теперь можно войти." });
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
                OrderId = Random.Shared.Next(111111111,999999999),
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

            foreach (var item in cartItems)
            {
                _cartService.RemoveFromCart(item.ProductId);
            }

            return View(order);
        }

        public IActionResult Profile()
        {
            var username = HttpContext.Session.GetString("Username");

            DataTable dt = new DataTable();
            dt = database.GetData($"select * from users where username = '{username}';");

            User user = new User(0, null, null, null, null, null, null, null);

            foreach (DataRow row in dt.Rows)
            {
                user.Id = Convert.ToInt32(row[0]);
                user.Login = row[1].ToString();
                user.Password = row[2].ToString();
                user.LastName = row[3].ToString();
                user.FirstName = row[4].ToString();
                user.MiddleName = row[5].ToString();
                user.Email = row[6].ToString();
                user.Phone = row[7].ToString();
            }
            return View(user);
        }

        [HttpPost]
        public IActionResult EditProfile(string lastName, string firstName, string middleName, string phone, string email, string login, string password)
        {
            var username = HttpContext.Session.GetString("Username");
            DataTable dt = new DataTable();
            dt = database.GetData($"select * from users where username = '{username}';");

            User user = new User(0, null, null, null, null, null, null, null);

            foreach (DataRow row in dt.Rows)
            {
                user.Id = Convert.ToInt32(row[0]);
                user.Login = row[1].ToString();
                user.Password = row[2].ToString();
                user.LastName = row[3].ToString();
                user.FirstName = row[4].ToString();
                user.MiddleName = row[5].ToString();
                user.Email = row[6].ToString();
                user.Phone = row[7].ToString();
            }

            if (ModelState.IsValid)
            {
                user.LastName = lastName;
                user.FirstName = firstName;
                user.MiddleName = middleName;
                user.Phone = phone;
                user.Email = email;
                user.Login = login;
                user.Password = password;

                var addQuery = $"update users set lastname = '{user.LastName}', firstname = '{user.FirstName}', middlename = '{user.MiddleName}', phone = '{user.Phone}', email = '{user.Email}', username = '{user.Login}', password = '{user.Password}' where id = {user.Id}";
                database.GetData(addQuery);

                HttpContext.Session.SetString("Username", user.Login);
            }
            return View(user);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
