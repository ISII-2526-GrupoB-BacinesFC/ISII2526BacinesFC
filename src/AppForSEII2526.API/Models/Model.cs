namespace AppForSEII2526.API.Models
{
    //Title is unique for each instance of Movie
    [Index(nameof(Name), IsUnique = true)]
    public class Model
    {
        public Model()
        {
        }

        public Model(string name)
        {
            Name = name;
        }

        public int Id { get; set; }

        [StringLength(50, ErrorMessage = "Model name cannot be longer than 50 characters.", MinimumLength = 4)]
        public string Name { get; set; }

        //it assigns a value by default
        public IList<Device> Devices { get; set; } = new List<Device>();

    }
}