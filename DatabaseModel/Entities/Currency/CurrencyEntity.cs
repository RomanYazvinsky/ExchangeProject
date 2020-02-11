namespace DatabaseModel.Entities.Currency
{
    public class CurrencyEntity: Entity
    {
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public string CurrencySign { get; set; }
        public string CountryCode { get; set; }
    } // todo add field for link to update currency
}
