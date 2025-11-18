namespace ErtushevShop.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Image { get; set; }
        public int Price { get; set; }

        public Product(int id, string brand, string model, string descript, string category, string image, int price)
        {
            Id = id;
            Brand = brand;
            Model = model;
            Description = descript;
            Category = category;
            Image = image;
            Price = price;
        }

        public override string ToString()
        {
            return $"{Brand} {Model}";
        }
    }
}
