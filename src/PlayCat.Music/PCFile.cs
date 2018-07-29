namespace PlayCat.Music
{
    public class PCFile : IFile
    {
        public string Filename { get; set; }
        public string Extension { get; set; }
        public StorageType StorageType { get; set; }
        public double Duration { get; set; }
    }
}
