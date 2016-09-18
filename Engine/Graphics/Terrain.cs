using System;
using Engine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Graphics
{
    public class Terrain
    {
        protected class Cell
        {
            public byte TileType;
            public Rectangle Destination;
        }

        protected TerrainData _data;
        protected Cell[][] _map;
        protected Rectangle _region;
        protected Rectangle[] _tileSources;
        private int _viewCornerX;
        private int _viewCornerY;
        private int _viewHorizontalTilesCount;
        private int _viewVerticalTilesCount;

        public int TilesCountWidth { get; protected set; }

        public int TilesCountHeight { get; protected set; }

        /// <summary>
        /// Initializes and generates tiles map
        /// </summary>
        /// <param name="terrainData">terrain data</param>
        /// <param name="region">Region of tiles to generate</param>
        /// <param name="rawMapData">Data of generated map surface</param>
        public Terrain(TerrainData terrainData, Rectangle region, byte[] rawMapData)
        {
            _data = terrainData;
            _region = region;
            TilesCountWidth = _region.Width/_data.GameTileWidth;
            TilesCountHeight = _region.Height/_data.GameTileHeight;
            GenerateTileMap(rawMapData);
            _tileSources = new Rectangle[_data.TilesCount+1];
            for (var i = 1; i < _data.TilesCount+1; i++)
            {
                _tileSources[i] = GetTileRectangle(i-1);
            }
            //EngineGlobals.Camera.OnCameraMove += CalculateDrawRegion;
            //CalculateDrawRegion(null);
           // EngineGlobals.Camera.OnCameraMove += RecalculateDestinations;
        }

        //void RecalculateDestinations(object sender)
        //{
        //    for (var i = 0; i < _map.Length; i++)
        //    {
        //        for (var j = 0; j < _region.Height/_data.GameTileHeight; j++)
        //        {
        //            _map[i][j].Destination =
        //                EngineGlobals.Camera.Transform(new Rectangle(_region.X + _data.GameTileWidth*i,
        //                                                             _region.Y + _data.GameTileHeight*j,
        //                                                             _data.GameTileWidth,
        //                                                             _data.GameTileHeight));
        //        }
        //    }
        //}

        public void GenerateTileMap(byte[] rawMapData)
        {
            _map = new Cell[TilesCountWidth][];
            for (var x = 0; x < TilesCountWidth; x++)
            {
                _map[x] = new Cell[TilesCountHeight];
                for (var y = 0; y < TilesCountHeight; y++)
                {
                    _map[x][y] = new Cell
                                     {
                                         TileType = rawMapData[x + y * TilesCountWidth],
                                         Destination = new Rectangle(_region.X + _data.GameTileWidth*x,
                                                                     _region.Y + _data.GameTileHeight*y,
                                                                     _data.GameTileWidth,
                                                                     _data.GameTileHeight)
                                     };
                }

            }
           /* var set = new int[_data.TilesCount][];
            var p = 0;
            for (var i = 0; i < _data.TilesCount; i++)
            {
                set[i] = new int[2];
                set[i][0] = p;
                if (i + 1 != _data.TilesCount)
                    p += 100/_data.TilesCount;
                else
                    p = 100;
                set[i][1] = p;
            }
            var random = new Random();
            for (var i = 0; i < _map.Length; i++)
            {
                for (var j = 0; j < _region.Height/_data.GameTileHeight; j++)
                {
                    var r = random.Next(0, 100);
                    for (var k = 0; k < _data.TilesCount; k++)
                    {
                        if (r < set[k][0] || r > set[k][1]) continue;
                        _map[i][j] = new Cell
                                         {
                                             TileType = (byte) k,
                                             Destination =
                                                 new Rectangle(_region.X + _data.GameTileWidth*i,
                                                               _region.Y + _data.GameTileHeight*j, _data.GameTileWidth,
                                                               _data.GameTileHeight)
                                         };
                        break;
                    }
                    
                }
            }*/
        }

        public Vector2 TileToPoint(Point tPoint)
        {
            return new Vector2(_data.GameTileWidth * tPoint.X + _region.X,_data.GameTileHeight * tPoint.Y + _region.Y);
        }

        public Rectangle TileToRectangle(Rectangle tRectangle)
        {
            return new Rectangle(_data.GameTileWidth * tRectangle.X + _region.X, _data.GameTileHeight * tRectangle.Y + _region.Y, tRectangle.Width * _data.GameTileWidth, tRectangle.Height * tRectangle.Height);
        }

        public Point PointToTile(Vector2 point)
        {
            return new Point((int)Math.Ceiling((point.X - _region.X) / _data.GameTileWidth), (int)Math.Ceiling((point.Y - _region.Y) / _data.GameTileHeight));
        }

        public bool IsWalkable(Point point)
        {
            return (_map[point.X][point.Y].TileType != 0);
        }

        protected Rectangle GetTileRectangle(int tile)
        {
            var y = tile / (_data.Texture.Width / _data.TileWidth);
            y *= _data.TileHeight;
            var x = tile % (_data.Texture.Width / _data.TileWidth);
            x *= _data.TileWidth;
            return new Rectangle(x, y, _data.TileWidth, _data.TileHeight);
        }

        //protected void CalculateDrawRegion(object sender)
        //{
        //    if (EngineGlobals.Camera.Position.X < _region.X)
        //    {
        //        _viewCornerX = 0;
        //        _viewHorizontalTilesCount = (int)Math.Ceiling((EngineGlobals.Device.Viewport.Width +
        //                                   (double)(EngineGlobals.Camera.Position.X - _region.X)) / _data.GameTileWidth);
        //        if (_viewHorizontalTilesCount < 0)
        //            _viewHorizontalTilesCount = 0;
        //    }
        //    else
        //    {
        //        _viewCornerX = Math.Abs((EngineGlobals.Camera.Position.X + _region.X)/_data.GameTileWidth);
        //        _viewHorizontalTilesCount = (int)Math.Ceiling(_viewCornerX + (double)EngineGlobals.Device.Viewport.Width/_data.GameTileWidth);
                
        //    }
        //    if (_viewHorizontalTilesCount > _map.Length)
        //        _viewHorizontalTilesCount = _map.Length;
        //    if (EngineGlobals.Camera.Position.Y < _region.Y)
        //    {
        //        _viewCornerY = 0;
        //        _viewVerticalTilesCount = (int)Math.Ceiling((EngineGlobals.Device.Viewport.Height +
        //                                   (double)(EngineGlobals.Camera.Position.Y - _region.Y))/_data.GameTileHeight);
        //        if (_viewVerticalTilesCount < 0)
        //            _viewVerticalTilesCount = 0;
        //    }
        //    else
        //    {
        //        _viewCornerY = Math.Abs((EngineGlobals.Camera.Position.Y + _region.Y)/_data.TileHeight);
        //        _viewVerticalTilesCount = (int)Math.Ceiling(_viewCornerY + (double)EngineGlobals.Device.Viewport.Height/_data.GameTileHeight);
                
        //    }
        //    if (_viewVerticalTilesCount > _map[0].Length)
        //        _viewVerticalTilesCount = _map[0].Length;
        //}

        public void Draw()
        {
            for (var i = _viewCornerX; i < _viewHorizontalTilesCount; i++)
            {
                for (var j = _viewCornerY; j < _viewVerticalTilesCount; j++)
                {
                    if (_map[i][j].TileType != 0)
                        EngineGlobals.Batch.Draw(_data.Texture, _map[i][j].Destination, _tileSources[_map[i][j].TileType], Color.White, 0.0f, new Vector2(0,0), SpriteEffects.None, 1.0f);
                }
            }
        }


    }

    public class TerrainData
    {
        protected string _path;

        public string Path
        {
            set
            {
                _path = value;
                if (EngineGlobals.ContentCache != null)
                    Texture = EngineGlobals.ContentCache.Load<Texture2D>(_path);
            }
            get { return _path; }
        }

        public byte TilesCount;
        public int TileWidth;
        public int TileHeight;
        public int GameTileWidth;
        public int GameTileHeight;

        [ContentSerializerIgnore]
        public Texture2D Texture;
    }
}
