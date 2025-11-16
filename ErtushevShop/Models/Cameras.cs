namespace ErtushevShop.Models
{
    public class Cameras: Product
    {
        public string SensorSize { get; set; }
        public string ShutterSpeed { get; set; }
        public string Aperture { get; set; }
        public string IsoRange { get; set; }
        public string PhotoRes { get; set; }

        public Cameras(int id, string brand, string model, string descript, string category, string image, int price, string sensorsize, string shutterspeed, string aperture, string isorange, string photores)
            :base(id, brand, model, descript, category, image, price)
        {
            SensorSize = sensorsize;
            ShutterSpeed = shutterspeed;
            Aperture = aperture;
            IsoRange = isorange;
            PhotoRes = photores;
        }
    }
}
