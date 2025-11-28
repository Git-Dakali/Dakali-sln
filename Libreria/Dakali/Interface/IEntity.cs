namespace Dakali.Interface
{
    public interface IEntity
    {
        long Id { get; set; }
        
        string SearchString { get; set; }
    }
}
