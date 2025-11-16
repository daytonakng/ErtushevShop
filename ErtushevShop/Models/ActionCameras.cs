namespace ErtushevShop.Models
{
    public class ActionCameras: Product
    {
        public int ViewAngle { get; set; }
        public int Depth { get; set; }
        public string Fixation { get; set; }    
        public string AppManage { get; set; }
        public string IsoRange { get; set; }

        public ActionCameras(int id, string brand, string model, string descript, string category, string image, int price, int viewangle, int depth, string fixation, string appmanage, string isorange)
            : base(id, brand, model, descript, category, image, price)
        {
            ViewAngle = viewangle;
            Depth = depth;
            Fixation = fixation;
            AppManage = appmanage;
            IsoRange = isorange;
        }
    }
}
