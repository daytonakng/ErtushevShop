using ErtushevShop.Models;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Data;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Net;
using System.Numerics;
using System.Reflection;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

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

                if (!System.IO.File.Exists($"wwwroot/img/{row[0]}.jpg"))
                {
                    product.Image = "/img/default.png";
                }
                productList.Add(product);
            }

            return productList;
        }

        private void CheckAdmin()
        {
            DataTable dt = new DataTable();
            dt = database.GetData("select * from users where role = 'admin';");
            var sessionUser = HttpContext.Session.GetString("Username");
            if (dt.Rows.Count > 0)
            {
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
                        phone: row[7].ToString(),
                        role: row[8].ToString()
                        );
                    if (sessionUser == user.Login)
                    {
                        ViewBag.Admin = true;
                        break;
                    }
                    else ViewBag.Admin = false;
                }
            }
            else ViewBag.Admin = false;
        }

        public IActionResult Index(string searchTerm = null, string selectedCategory = null, string selectedBrand = null, string sortBy = null)
        {
            //HttpContext.Session.SetString("Username", "Daytona");
            CheckAdmin();

            List<Product> products = GetProductsFromDB();

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

        public IActionResult EditProduct(int id)
        {
            CheckAdmin();

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

                    if (!System.IO.File.Exists($"wwwroot/img/{row[0]}.jpg"))
                    {
                        camera.Image = "/img/default.png";
                    }

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

                    if (!System.IO.File.Exists($"wwwroot/img/{row[0]}.jpg"))
                    {
                        actioncamera.Image = "/img/default.png";
                    }

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

                    if (!System.IO.File.Exists($"wwwroot/img/{row[0]}.jpg"))
                    {
                        copter.Image = "/img/default.png";
                    }

                    Product product = copter;
                    return View(product);
                }
            }
            return View();
        }

        public IActionResult Details(int id)
        {
            CheckAdmin();

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

                    if (!System.IO.File.Exists($"wwwroot/img/{row[0]}.jpg"))
                    {
                        camera.Image = "/img/default.png";
                    }

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

                    if (!System.IO.File.Exists($"wwwroot/img/{row[0]}.jpg"))
                    {
                        actioncamera.Image = "/img/default.png";
                    }

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

                    if (!System.IO.File.Exists($"wwwroot/img/{row[0]}.jpg"))
                    {
                        copter.Image = "/img/default.png";
                    }

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
                    phone: row[7].ToString(),
                    role: row[8].ToString()
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
                phone: null,
                role: null
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

            int userId = 0;
            var username = HttpContext.Session.GetString("Username");

            if (username != null)
            {
                DataTable dt = new DataTable();
                dt = database.GetData($"select id from users where username = '{username}'");
                userId = Convert.ToInt32(dt.Rows[0][0]);
            }

            var order = new Order
            {
                OrderId = Random.Shared.Next(111111111, 999999999),
                UserId = userId,
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

            var addQuery = $"insert into orders values ({order.OrderId}, '{order.LastName}', '{order.FirstName}', '{order.MiddleName}', '{order.Phone}', '{order.Email}', '{order.Address}', {order.TotalAmount}, '{order.CreatedAt.ToShortDateString()}', {order.UserId})";
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

        public IActionResult EditUser(int id)
        {
            CheckAdmin();

            List<User> users = GetUsersFromDb();

            ViewBag.Roles = users
                .Select(p => p.Role)
                .Distinct()
                .ToList();

            User user = users.Where(u => u.Id == id).FirstOrDefault();

            return View(user);
        }

        public IActionResult Profile()
        {
            CheckAdmin();

            var username = HttpContext.Session.GetString("Username");

            List<User> users = GetUsersFromDb();
            User user = users.Where(u => u.Login ==  username).FirstOrDefault();

            return View(user);
        }

        [HttpPost]
        public IActionResult EditProfile(string lastName, string firstName, string middleName, string phone, string email, string login, string password)
        {
            var username = HttpContext.Session.GetString("Username");

            List<User> users = GetUsersFromDb();
            User user = users.Where(u => u.Login == username).FirstOrDefault();

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

        [HttpPost]
        public IActionResult SaveUser(int id, string lastName, string firstName, string middleName, string phone, string email, string login, string role)
        {
            List<User> users = GetUsersFromDb();
            User user = users.Where(u => u.Id == id).FirstOrDefault();

            if (ModelState.IsValid)
            {
                user.LastName = lastName;
                user.FirstName = firstName;
                user.MiddleName = middleName;
                user.Phone = phone;
                user.Email = email;
                user.Login = login;
                user.Role = role;

                var addQuery = $"update users set lastname = '{user.LastName}', firstname = '{user.FirstName}', middlename = '{user.MiddleName}', phone = '{user.Phone}', email = '{user.Email}', username = '{user.Login}', role = '{user.Role}' where id = {user.Id}";
                database.GetData(addQuery);
            }
            return View(user);
        }

        [HttpPost]
        public IActionResult SaveProduct(int id, string brand, string model, string description, string category, int price, string sensorsize, string shutterspeed, string aperture, string isorange, string photores, int speed, int height, int depth, int duration, string fixation, int viewangle, string appmanage)
        {
            if (id / 100000 == 4)
            {
                Cameras camera = new Cameras(id, brand, model, description, category, null, price, sensorsize, shutterspeed, aperture, isorange, photores);
                Product product = camera;

                if (ModelState.IsValid)
                {
                    var addQuery = $"update cameras set brand = '{brand}', model = '{model}', descript = '{description}', category = '{category}', price = {price}, sensorsize = '{sensorsize}', shutterspeed = '{shutterspeed}', isorange = '{isorange}', photores = '{photores}' where id = {id}";
                    database.GetData(addQuery);
                }

                return View(product);
            }
            if (id / 100000 == 7)
            {
                ActionCameras actioncamera = new ActionCameras(id, brand, model, description, category, null, price, viewangle, depth, fixation, appmanage, isorange);
                Product product = actioncamera;

                if (ModelState.IsValid)
                {
                    var addQuery = $"update actioncameras set brand = '{brand}', model = '{model}', descript = '{description}', category = '{category}', price = {price}, viewangle = {viewangle}, depthcam = {depth}, fixation = '{fixation}', appmanage = '{appmanage}', isorange = '{isorange}' where id = {id}";
                    database.GetData(addQuery);
                }

                return View(product);
            }
            if (id / 100000 == 9)
            {
                Copters copter = new Copters(id, brand, model, description, category, null, price, speed, height, duration, appmanage, photores);
                Product product = copter;

                if (ModelState.IsValid)
                {
                    var addQuery = $"update copters set brand = '{brand}', model = '{model}', descript = '{description}', category = '{category}', price = {price}, speed = {speed}, height = {height}, duration = {duration}, appmanage = '{appmanage}', photores = '{photores}' where id = {id}";
                    database.GetData(addQuery);
                }

                return View(product);
            }

            TempData["ReturnUrl"] = Request.Path + Request.QueryString;

            return View();
        }

        public IActionResult RemoveProduct(int id)
        {
            if (id / 100000 == 4)
            {
                var addQuery = $"delete from cameras where id = {id}";
                database.GetData(addQuery);
            }
            if (id / 100000 == 7)
            {
                var addQuery = $"delete from actioncameras where id = {id}";
                database.GetData(addQuery);
            }
            if (id / 100000 == 9)
            {
                var addQuery = $"delete from copters where id = {id}";
                database.GetData(addQuery);
            }
            var returnUrl = TempData["ReturnUrl"]?.ToString();
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("ManageProducts", "Home");
        }

        public IActionResult RemoveUser(int id)
        {
            var delQuery = $"delete from users where id = {id}";
            database.GetData(delQuery);

            var returnUrl = TempData["ReturnUrl"]?.ToString();
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("ManageUsers", "Home");
        }

        public IActionResult ManageProducts()
        {
            CheckAdmin();
            List<Product> products = GetProductsFromDB();

            TempData["ReturnUrl"] = Request.Path + Request.QueryString;

            products = products.OrderBy(p => p.Id).ToList();

            return View(products);
        }

        public IActionResult ManageUsers()
        {
            CheckAdmin();
            List<User> users = GetUsersFromDb();

            TempData["ReturnUrl"] = Request.Path + Request.QueryString;

            users = users.OrderBy(u => u.Id).ToList();

            return View(users);
        }

        public IActionResult AddProduct()
        {
            CheckAdmin();

            List<Product> products = GetProductsFromDB();
            ViewBag.Categories = products
                .Select(p => p.Category)
                .Distinct()
                .ToList();

            return View(products);
        }

        [HttpPost]
        public JsonResult NewProduct(int id, string brand, string model, string description, string category, int price, string sensorsize, string shutterspeed, string aperture, string isorange, string photores, int speed, int height, int depth, int duration, string fixation, int viewangle, string appmanage)
        {
            if (id / 100000 == 4)
            {
                Cameras camera = new Cameras(
                    id: id,
                    brand: brand,
                    model: model,
                    descript: description,
                    category: category,
                    image: "/img/default.png",
                    price: price,
                    sensorsize: sensorsize,
                    shutterspeed: shutterspeed,
                    aperture: aperture,
                    isorange: isorange,
                    photores: photores
                    );

                var addQuery = $"insert into cameras values ({id}, '{category}', '{brand}', '{model}', '{description}', {price}, '{sensorsize}', '{shutterspeed}', '{aperture}', '{isorange}', '{photores}');";
                database.GetData(addQuery);
            }
            else if (id / 100000 == 7)
            {
                ActionCameras actioncamera = new ActionCameras(
                    id: id,
                    brand: brand,
                    model: model,
                    descript: description,
                    category: category,
                    image: "/img/default.png",
                    price: price,
                    viewangle: viewangle,
                    depth: depth,
                    fixation: fixation,
                    appmanage: appmanage,
                    isorange: isorange
                    );

                var addQuery = $"insert into actioncameras values ({id}, '{category}', '{brand}', '{model}', '{description}', {price}, '{viewangle}', '{depth}', '{fixation}', '{appmanage}', '{isorange}');";
                database.GetData(addQuery);
            }
            else if (id / 100000 == 9)
            {
                Copters copter = new Copters(
                    id: id,
                    brand: brand,
                    model: model,
                    descript: description,
                    category: category,
                    image: "/img/default.png",
                    price: price,
                    speed: speed,
                    height: height,
                    duration: duration,
                    appmanage: appmanage,
                    photores: photores
                    );

                var addQuery = $"insert into copters values ({id}, '{category}', '{brand}', '{model}', '{description}', {price}, {speed}, {height}, {duration}, '{appmanage}', '{photores}');";
                database.GetData(addQuery);
            }

            return Json(new { success = true, message = "Товар был успешно добавлен!" });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
