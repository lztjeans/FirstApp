namespace FirstApp.Constants
{
    public class DataNotFoundException : Exception
    {
        public DataNotFoundException(string id) : base($"No product found with Id: {id}")
        {
        }
    }
}
