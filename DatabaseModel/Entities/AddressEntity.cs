namespace DatabaseModel.Entities
{
    public class AddressEntity: Entity
    {
        public string CountryCode { get; set; }
        public string Address { get; set; }
        public string PostIndex { get; set; }
    }
}
