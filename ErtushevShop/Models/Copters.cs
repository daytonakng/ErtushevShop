namespace ErtushevShop.Models
{
    public class Copters: Product
    {
        public int Speed { get; set; }
        public int Height { get; set; }
        public int Duration { get; set; }
        public string AppManage { get; set; }
        public string PhotoRes { get; set; }

        public Copters(int id, string brand, string model, string descript, string category, string image, int price, int speed, int height, int duration, string appmanage, string photores)
            : base(id, brand, model, descript, category, image, price)
        {
            Speed = speed;
            Height = height;
            Duration = duration;
            AppManage = appmanage;
            PhotoRes = photores;
        }
    }
}
