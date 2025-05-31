namespace TodoList.Dtos
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int CategoryId { get; set; }
        public string CategoryPath { get; set; } // Full path like "Electronics > Computers > Samsung Screens"
    }

}
