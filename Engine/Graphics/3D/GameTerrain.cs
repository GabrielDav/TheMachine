using System;
using System.Collections.Generic;
using Engine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Graphics._3D
{

    public class TerrainData
    {
        protected GameTexture _tileDataTexture;
        protected int[] _mapData;

        public GameTexture TerrainMap
        {
            set
            {
                _tileDataTexture = value;
                var colorData = new Color[_tileDataTexture.Data.Width*_tileDataTexture.Data.Height];
                _tileDataTexture.Data.GetData(colorData);
                TileData = new byte[_tileDataTexture.Data.Width][];
                for (var x = 0; x < _tileDataTexture.Data.Width; x++)
                {
                    TileData[x] = new byte[_tileDataTexture.Data.Height];
                    for (var y = 0; y < _tileDataTexture.Data.Height; y++)
                    {
                        if (colorData[y * _tileDataTexture.Data.Width + x] == Color.Red)
                        {
                            TileData[x][y] = 1;
                        }
                        else if (colorData[y * _tileDataTexture.Data.Width + x] == Color.Blue)
                        {
                            TileData[x][y] = 2;
                        }
                    }
                }
            }
            get { return _tileDataTexture; }
        }
        public GameTexture TilesTexture;
        public int Width;
        public int Height;
        public int TileSize;
        public int Tiles;
        public int TextureTileWidth;
        public int TextureTileHeight;
        public byte[][] TileData { get; protected set; }


        /// <summary>
        /// Gets texture coordinates from tile id
        /// </summary>
        /// <param name="id">Tile id</param>
        /// <returns>Vector4 where x - X left, y - Y top, z - X right, w - Y bottom</returns>
        public Vector4 GetTextureCoord(byte id)
        {
            var y = id / (TilesTexture.Data.Width / TextureTileWidth);
            y *= TextureTileHeight;
            var x = id % (TilesTexture.Data.Width / TextureTileWidth);
            x *= TextureTileWidth;
            return new Vector4(x / (float)TilesTexture.Data.Width, y / (float)TilesTexture.Data.Height, (x + TextureTileWidth) / (float)TilesTexture.Data.Width, (y + (float)TextureTileHeight) / TilesTexture.Data.Height);
        }

        
    }

    public class GameTerrain : IDisposable
    {
        protected class TilesCluster : IDisposable
        {
            public static IndexBuffer IndexBuffer;
            public VertexBuffer VertexBuffer;

            protected TerrainData _data;

            public TilesCluster(TerrainData data)
            {
                _data = data;
            }

            /// <summary>
            /// Creates Tiles Cluster with Verticies
            /// </summary>
            /// <param name="width">Width of cluster(in tiles)</param>
            /// <param name="height">Height of cluster(in tiles)</param>
            /// <param name="startX">Cluster start position X(Tile position)</param>
            /// <param name="startY">Cluster start position Y(Tile position)</param>
            public void Build(int width, int height, int startX, int startY)
            {

                var vertices = new VertexPositionTexture[width * height * 6];
                var counter = 0;
                for (var y = startY; y < startY + height; y++)
                    for (var x = startX; x < startX + width; x++)
                    {
                        var posX = (x) * _data.TileSize;
                        var posY = (y) * -_data.TileSize;
                        var textureCoord = _data.GetTextureCoord(_data.TileData[x][y]);

                        vertices[counter++] = new VertexPositionTexture(new Vector3(posX + _data.TileSize, posY - _data.TileSize, 0), new Vector2(textureCoord.Z, textureCoord.W)); //x+1, -y   0
                        vertices[counter++] = new VertexPositionTexture(new Vector3(posX, posY, 0), new Vector2(textureCoord.X, textureCoord.Y)); //x,-y   1
                        vertices[counter++] = new VertexPositionTexture(new Vector3(posX + _data.TileSize, posY, 0), new Vector2(textureCoord.Z, textureCoord.Y));//x, -y-1     2

                        vertices[counter++] = new VertexPositionTexture(new Vector3(posX + _data.TileSize, posY - _data.TileSize, 0), new Vector2(textureCoord.Z, textureCoord.W)); //x+1, -y   0
                        vertices[counter++] = new VertexPositionTexture(new Vector3(posX, posY - _data.TileSize, 0), new Vector2(textureCoord.X, textureCoord.W)); //x,-y   1
                        vertices[counter++] = new VertexPositionTexture(new Vector3(posX, posY, 0), new Vector2(textureCoord.X, textureCoord.Y));//x, -y-1     2

                    }
                VertexBuffer = new VertexBuffer(EngineGlobals.Device,
                                                VertexPositionTexture.VertexDeclaration, vertices.Length,
                                                BufferUsage.WriteOnly);
                VertexBuffer.SetData(vertices);


            }

            /// <summary>
            /// Set default Indices for all tiles clusters
            /// </summary>
            /// <param name="width">Width of cluster(in tiles)</param>
            /// <param name="height">Height of cluster(in tiles)</param>
            public static void SetupIndices(short width, short height)
            {
                var indices = new short[width * height * 6];
                var counter = 0;
                for (short y = 0; y < height; y++)
                {
                    for (short x = 0; x < width; x++)
                    {
                        var start = (short)(x * 6 + y * 6 * width);
                        indices[counter++] = start;
                        indices[counter++] = (short)(start + 1);
                        indices[counter++] = (short)(start + 2);

                        indices[counter++] = (short)(start + 3);
                        indices[counter++] = (short)(start + 4);
                        indices[counter++] = (short)(start + 5);
                    }
                }
                IndexBuffer = new IndexBuffer(EngineGlobals.Device, typeof(short), indices.Length,
                                              BufferUsage.WriteOnly);
                IndexBuffer.SetData(indices);
            }

            public void Dispose()
            {
                _data = null;
                VertexBuffer.Dispose();
            }
        }

        protected BasicEffect _terrainShader;
        protected TerrainData _data;
        protected TilesCluster[][] _clusters;
        protected const int _clusterWidth = 8;
        protected const int _clusterHeight = 6;
        protected Point[] _clusterToDraw;


        public GameTerrain(TerrainData data, BasicEffect shader)
        {
            _data = data;
            _terrainShader = shader;
            _terrainShader.TextureEnabled = true;
            _terrainShader.LightingEnabled = false;
            _terrainShader.VertexColorEnabled = false;
            _terrainShader.Texture = _data.TilesTexture.Data;
        }

        public void Build()
        {
            TilesCluster.SetupIndices(_clusterWidth, _clusterHeight);
            if (_data.Width % _clusterWidth != 0)
            {
                throw new Exception("Tiles map width is incorrect");
            }
            if (_data.Height % _clusterHeight != 0)
            {
                throw new Exception("Tiles map height is incorrect");
            }
            _clusters = new TilesCluster[_data.Width / _clusterWidth][];
            for (var x = 0; x < _data.Width; x += _clusterWidth)
            {
                _clusters[x / _clusterWidth] = new TilesCluster[_data.Height / _clusterHeight];
                for (var y = 0; y < _data.Height; y += _clusterHeight)
                {
                    _clusters[x / _clusterWidth][y / _clusterHeight] = new TilesCluster(_data);
                    _clusters[x / _clusterWidth][y / _clusterHeight].Build(_clusterWidth, _clusterHeight, x, y);
                }
            }
        }

        public void CameraUpdate(Rectangle frustum)
        {
            var visibleClusters = new List<Point>();
            var cw = (int)Math.Ceiling(frustum.X / (double)frustum.Width);
            var ch = (int)Math.Ceiling(frustum.X / (double)frustum.Height);
            var lengthW = (int)Math.Ceiling((_data.TileSize - (cw * _data.TileSize - frustum.X)) / (double)_data.TileSize);
            var lengthH = (int)Math.Ceiling((_data.TileSize - (ch * _data.TileSize - frustum.Y)) / (double)_data.TileSize);
            for (var i = cw; i < cw + lengthW; i++)
            {
                for (var j = ch; j < ch + lengthH; j++)
                {
                    visibleClusters.Add(new Point(i - 1, j - 1));
                }
            }
            _clusterToDraw = visibleClusters.ToArray();
        }

        public void Draw(Matrix viewMatrix, Matrix projectionMatrix)
        {
            EngineGlobals.Device.Indices = TilesCluster.IndexBuffer;
            
            _terrainShader.World = Matrix.Identity;
            //_terrainShader.Projection = EngineGlobals.Camera3D.Projection;
            //_terrainShader.View = EngineGlobals.Camera3D.View;
            for (var x = 0; x < _clusters.Length; x++)
            {
                for (var y = 0; y < _clusters[x].Length; y++)
                {

                    foreach (var pass in _terrainShader.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        EngineGlobals.Device.SetVertexBuffer(_clusters[x][y].VertexBuffer);

                        EngineGlobals.Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _clusters[x][y].VertexBuffer.VertexCount, 0, _clusters[x][y].VertexBuffer.VertexCount / 3);
                    }

                }
            }
        }

        public void Dispose()
        {
            for (var x = 0; x < _clusters.Length; x++)
            {
                for (var y = 0; y < _clusters[x].Length; y++)
                {
                    _clusters[x][y].Dispose();
                }
            }
            TilesCluster.IndexBuffer.Dispose();
            _clusters = null;
            _terrainShader.Dispose();
            _data.TerrainMap.Data.Dispose();
            _data = null;
        }
    }
}
