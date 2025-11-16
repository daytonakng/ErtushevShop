using System.Data;
using System.Diagnostics;
using ErtushevShop.Models;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;

namespace ErtushevShop.Controllers
{
    public class HomeController : Controller
    {
        Database database = new Database();

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index(string searchTerm = null, string selectedCategory = null, string sortBy = null)
        {
            DataTable dtGetData = new DataTable();

            dtGetData = database.GetData("SELECT id, brand, model, category, descript, price FROM cameras UNION ALL SELECT id, brand, model, category, descript, price FROM actioncameras UNION ALL SELECT id, brand, model, category, descript, price FROM copters;");

            List<Product> products = new List<Product>();

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
                products.Add(product);
            }

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

        public IActionResult Privacy()
        {
            return View();
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
            return View();
        }

        public IActionResult Favorite()
        {
            return View();
        }

        public IActionResult Profile()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Test()
        {
            return Content("<h1>Ertushev</h1>", "text/html");
        }
    }
}
