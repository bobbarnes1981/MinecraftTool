namespace MinecraftLibrary
{
    public class Chunk
    {
        public int Length { get; set; }
        public CompressionScheme CompressionScheme { get; set; }
        public Tag Data { get; set; }

        public Chunk(int length, CompressionScheme compressionScheme, Tag data)
        {
            Length = length;
            CompressionScheme = compressionScheme;
            Data = data;
        }
    }
}
