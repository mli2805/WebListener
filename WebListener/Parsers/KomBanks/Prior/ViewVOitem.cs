namespace WebListener
{
    public class ViewVOitem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Code { get; set; }
        public string Iso { get; set; }
        public long AriseTime { get; set; }
        public int ChannelId { get; set; }
        public double Buy { get; set; }
        public double Sell { get; set; }
        public double? Lowerlimit { get; set; }
        public double? Upperlimit { get; set; }
        public int Quantity { get; set; }
    }
}