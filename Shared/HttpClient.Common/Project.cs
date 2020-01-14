namespace HttpClient.Common
{
    public class Project
    {
        public Project(string name, string url, int stars)
        {
            Name = name;
            Url = url;
            Stars = stars;
        }

        public string Name { get; }

        public int Stars { get; }

        public string Url { get; }
    }
}