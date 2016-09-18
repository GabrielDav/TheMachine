
namespace GameLibrary.GridBuilder
{

    public enum TileType
    {
        Normal,
        Hardcore
    }

    public interface ITileBuilder
    {
        void BuildTile(int number, int x, int y, int width, int height, TileType? type);

        void Init();
    }
}
