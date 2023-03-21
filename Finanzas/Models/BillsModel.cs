namespace Finanzas.Models
{
    public class Bills
    {
        public int idBill { get; set; }
        public int idIcons { get; set; }
        public string Name { get; set; }
        public double Cost { get; set; }
        public string PaymentDate { get; set; }
        public string Paid { get; set; }
        public int idMonth { get; set; }
        public int idUser { get; set; }
        public int idCategory { get; set; }
        public int idPaymentMethod { get; set; }
    }
}