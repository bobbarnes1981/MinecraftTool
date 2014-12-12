namespace MinecraftLibrary
{
    public class Region
    {
        public ChunkLocation[] ChunkLocations { get; set; }

        public int[] TimeStamps { get; set; }

        public Chunk[] Chunks { get; set; }

        public Region(int numberOfChunks)
        {
            ChunkLocations = new ChunkLocation[numberOfChunks];
            TimeStamps = new int[numberOfChunks];
            Chunks = new Chunk[numberOfChunks];
        }
    }
}
