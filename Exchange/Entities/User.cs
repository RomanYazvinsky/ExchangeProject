namespace Exchange.Entities
{
    public enum Role
    {
        Administrator,
        Operator,
        Watcher,
        Customer
    }

    public class User: Entity
    {
        public Role Role { get; set; }
    }
}