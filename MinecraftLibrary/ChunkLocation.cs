namespace MinecraftLibrary
{
    public class ChunkLocation
    {
        public int Offset { get; set; }

        public byte Sectors { get; set; }

        public ChunkLocation(int offset, byte sectors)
        {
            Offset = offset;
            Sectors = sectors;
        }
    }
}
