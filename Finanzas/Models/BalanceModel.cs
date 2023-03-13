namespace Finanzas.Models
{
    public class Balance
    {
        public int idBalance { get; set; }
        public string? Income { get; set; }
        public string? Expenses { get; set; }
        public string? Remaining { get; set; }
        public int idUser { get; set; }
        public int idMonth { get; set; }
    }
}