using System.Collections.Generic;
using Engine.Core;
using Engine.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Engine.Mechanics
{
    //public class Level
    //{
    //    public string Name;
    //    public string Id;
    //    public int Width;
    //    public int Height;
    //    [ContentSerializer(ElementName = "TileSet")] public string TileSetPath;
    //    [ContentSerializer(ElementName = "UnitTypes")] public string[] UnitTypePaths;
    //    [ContentSerializerIgnore] public Dictionary<string, UnitType> UnitTypes;
    //    [ContentSerializerIgnore] protected TerrainData _terrainData;
    //    [ContentSerializerIgnore] public Terrain Surface;
    //    [ContentSerializerIgnore] public Unit[] Units;

    //    [ContentSerializerIgnore]
    //    public Unit[,] MapData { get; protected set; }

    //    [ContentSerializerIgnore] public Unit PlayerHero;

    //    public void Build()
    //    {
    //        UnitTypes = new Dictionary<string, UnitType>();
    //        for (var i = 0; i < UnitTypePaths.Length; i++)
    //        {
    //            var type = EngineGlobals.Content.Load<UnitType>(UnitTypePaths[i]);
    //            UnitTypes[type.TypeName] = type;
    //        }
    //        _terrainData = EngineGlobals.Content.Load<TerrainData>(TileSetPath);
    //        /*var storage = IsolatedStorageFile.GetUserStoreForApplication();
    //        if (!storage.DirectoryExists("cache"))
    //            storage.CreateDirectory("cache");
    //        var rawSurfaceData = storage.FileExists(Id) ? LoadSurfaceData(storage) : GenerateSurfaceData(storage);*/
    //        if (!EngineGlobals.Storage.DirectoryExists("cache"))
    //            EngineGlobals.Storage.CreateDirecotry("cache");
    //        var rawSurfaceData = EngineGlobals.Storage.FileExists(Id) ? LoadSurfaceData() : GenerateSurfaceData();
    //        Surface = new Terrain(_terrainData, new Rectangle(0, 0, Width, Height), rawSurfaceData);

    //        Units = new Unit[2];
    //        Units[0] = new Unit(UnitTypes["Hero"], Player.Human, new Point(2, 2));
    //        Units[0].LocationChanged += UnitLocationChanged;

    //        PlayerHero = Units[0];
    //        Units[1] = new Unit(UnitTypes["EvillBall"], Player.Hostile, new Point(7, 3));
    //        Units[1].OnDeath += UnitDies;
    //        Units[1].LocationChanged += UnitLocationChanged;
    //        MapData = new Unit[Surface.TilesCountWidth,Surface.TilesCountHeight];
    //        MapData[2, 2] = Units[0];
    //        MapData[7, 3] = Units[1];
    //    }

    //    private void UnitLocationChanged(object sender, Point oldLocation)
    //    {
    //        var u = (Unit) sender;
    //        MapData[oldLocation.X, oldLocation.Y] = null;
    //        MapData[u.Location.X, u.Location.Y] = u;
    //    }

    //    private void UnitDies(object sender)
    //    {
    //        var u = (Unit) sender;
    //        MapData[u.Location.X, u.Location.Y] = null;
    //    }

    //    public void Draw()
    //    {
    //        Surface.Draw();
    //        for (var i = 0; i < Units.Length; i++)
    //        {
    //            Units[i].Draw();
    //        }
    //    }

    //    protected byte[] LoadSurfaceData()
    //    {
    //        var reader = EngineGlobals.Storage.OpenFileBinaryRead("cache\\" + Id);
    //        var map = reader.ReadBytes((Width/_terrainData.GameTileWidth)*(Height/_terrainData.GameTileHeight));
    //        reader.Close();
    //        EngineGlobals.Storage.CloseStream();
    //        return map;
    //    }

    //    protected byte[] GenerateSurfaceData()
    //    {
    //        var map = new byte[]
    //                      {
    //                          1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0,
    //                          1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1,
    //                          1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1,
    //                          1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1,
    //                          1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
    //                          1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1,
    //                          1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1,
    //                          1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1
    //                      };
    //        /*var map = new byte[]
    //                      {
    //                          1, 1, 1,
    //                          1, 1, 0,
    //                          1, 1, 0
    //                      };*/
    //        var writer = EngineGlobals.Storage.OpenFileBinaryWrite("cache\\" + Id);
    //        writer.Write(map);
    //        writer.Close();
    //        EngineGlobals.Storage.CloseStream();
    //        return map;
    //    }

    //}
}
