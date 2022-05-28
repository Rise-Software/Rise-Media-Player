namespace Rise.App.Models
{
    public interface IEnumerationDestination<in TEnumeration>
    {
        void AddFromEnumeration(TEnumeration enumeration);
    }
}
