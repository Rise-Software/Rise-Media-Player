namespace Rise.Models
{
    public class Notification : DbObject, System.IEquatable<Notification>
    {
        public string Title, Description, Icon;

        public bool Equals(Notification other)
        {
            return Title == other.Title &&
                   Description == other.Description &&
                   Icon == other.Icon;
        }
    }
}
