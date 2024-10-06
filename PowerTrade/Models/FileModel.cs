namespace PowerTrade.Models
{
    public record FileModel
    {
        public string Path { get; set; }

        public string Name { get; set; }

        public string FilePathName => System.IO.Path.Combine(Path, Name);
    }
}