// Type: Microsoft.Xna.Framework.Graphics.SpriteBatch
// Assembly: Microsoft.Xna.Framework.Graphics, Version=4.0.0.0, Culture=neutral, PublicKeyToken=842cf8be1de50553
// Assembly location: C:\Program Files (x86)\Microsoft XNA\XNA Game Studio\v4.0\References\Windows\x86\Microsoft.Xna.Framework.Graphics.dll

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Xna.Framework.Graphics
{
    public class DrawBatch
    {
        private static readonly float[] xCornerOffsets = new float[4]
    {
      0.0f,
      1f,
      1f,
      0.0f
    };
        private static readonly float[] yCornerOffsets = new float[4]
    {
      0.0f,
      0.0f,
      1f,
      1f
    };
        private static Vector2 vector2Zero = Vector2.Zero;
        private static Rectangle? nullRectangle = new Rectangle?();
        private VertexPositionColorTexture[] outputVertices = new VertexPositionColorTexture[8192];
        private DrawBatch.SpriteInfo[] spriteQueue = new DrawBatch.SpriteInfo[2048];
        private const int MaxBatchSize = 2048;
        private DynamicVertexBuffer vertexBuffer;
        private DynamicIndexBuffer indexBuffer;
        private int vertexBufferPosition;
        private Effect spriteEffect;
        private EffectParameter effectMatrixTransform;
        private SpriteSortMode spriteSortMode;
        private BlendState blendState;
        private DepthStencilState depthStencilState;
        private RasterizerState rasterizerState;
        private SamplerState samplerState;
        private Effect customEffect;
        private Matrix transformMatrix;
        private bool inBeginEndPair;
        private int spriteQueueCount;
        private Texture2D[] spriteTextures;
        private int[] sortIndices;
        private DrawBatch.SpriteInfo[] sortedSprites;
        private DrawBatch.TextureComparer textureComparer;
        private DrawBatch.BackToFrontComparer backToFrontComparer;
        private DrawBatch.FrontToBackComparer frontToBackComparer;
        private GraphicsDevice graphicsDevice;
        private int spriteBeginCount;
        private int spriteImmediateBeginCount;

        static DrawBatch()
        {
        }

        public DrawBatch(GraphicsDevice graphicsDevice)
        {
            if (graphicsDevice == null)
                throw new ArgumentNullException("Graphics device is null");
            
           // this.spriteEffect = new Effect(); TODO: add effect code
            this.effectMatrixTransform = this.spriteEffect.Parameters["MatrixTransform"];
            this.ConstructPlatformData();
        }

        private void ConstructPlatformData()
        {
            this.AllocateBuffers();
        }

        private void DisposePlatformData()
        {
            if (this.vertexBuffer != null)
                this.vertexBuffer.Dispose();
            if (this.indexBuffer == null)
                return;
            this.indexBuffer.Dispose();
        }

        private void AllocateBuffers()
        {
            if (this.vertexBuffer == null || this.vertexBuffer.IsDisposed)
            {
                this.vertexBuffer = new DynamicVertexBuffer(graphicsDevice, typeof(VertexPositionColorTexture), 8192, BufferUsage.WriteOnly);
                this.vertexBufferPosition = 0;
                this.vertexBuffer.ContentLost += (EventHandler<EventArgs>)((sender, e) => this.vertexBufferPosition = 0);
            }
            if (this.indexBuffer != null && !this.indexBuffer.IsDisposed)
                return;
            this.indexBuffer = new DynamicIndexBuffer(graphicsDevice, typeof(short), 12288, BufferUsage.WriteOnly);
            ((IndexBuffer)this.indexBuffer).SetData<short>(DrawBatch.CreateIndexData());
            this.indexBuffer.ContentLost += (EventHandler<EventArgs>)((sender, e) => indexBuffer.SetData(CreateIndexData()));
        }

        private static short[] CreateIndexData()
        {
            short[] numArray = new short[12288];
            for (int index = 0; index < 2048; ++index)
            {
                numArray[index * 6] = (short)(index * 4);
                numArray[index * 6 + 1] = (short)(index * 4 + 1);
                numArray[index * 6 + 2] = (short)(index * 4 + 2);
                numArray[index * 6 + 3] = (short)(index * 4);
                numArray[index * 6 + 4] = (short)(index * 4 + 2);
                numArray[index * 6 + 5] = (short)(index * 4 + 3);
            }
            return numArray;
        }

        private void SetPlatformRenderState()
        {
            this.AllocateBuffers();
            graphicsDevice.SetVertexBuffer((VertexBuffer)this.vertexBuffer);
            graphicsDevice.Indices = (IndexBuffer)this.indexBuffer;
        }

        //TODO: fix unsafe
        private void PlatformRenderBatch(Texture2D texture, DrawBatch.SpriteInfo[] sprites, int offset, int count)
        {
        }

        /*
        private unsafe void PlatformRenderBatch(Texture2D texture, SpriteBatch.SpriteInfo[] sprites, int offset, int count)
        {
            float num1 = 1f / (float)texture.Width;
            float num2 = 1f / (float)texture.Height;
            while (count > 0)
            {
                SetDataOptions options = SetDataOptions.NoOverwrite;
                int num3 = count;
                if (num3 > 2048 - this.vertexBufferPosition)
                {
                    num3 = 2048 - this.vertexBufferPosition;
                    if (num3 < 256)
                    {
                        this.vertexBufferPosition = 0;
                        options = SetDataOptions.Discard;
                        num3 = count;
                        if (num3 > 2048)
                            num3 = 2048;
                    }
                }
                fixed (SpriteBatch.SpriteInfo* spriteInfoPtr1 = &sprites[offset])
                fixed (VertexPositionColorTexture* positionColorTexturePtr1 = &this.outputVertices[0])
                {
                    SpriteBatch.SpriteInfo* spriteInfoPtr2 = spriteInfoPtr1;
                    VertexPositionColorTexture* positionColorTexturePtr2 = positionColorTexturePtr1;
                    for (int index1 = 0; index1 < num3; ++index1)
                    {
                        float num4;
                        float num5;
                        if ((double)spriteInfoPtr2->Rotation != 0.0)
                        {
                            num4 = (float)Math.Cos((double)spriteInfoPtr2->Rotation);
                            num5 = (float)Math.Sin((double)spriteInfoPtr2->Rotation);
                        }
                        else
                        {
                            num4 = 1f;
                            num5 = 0.0f;
                        }
                        float num6 = (double)spriteInfoPtr2->Source.Z != 0.0 ? spriteInfoPtr2->Origin.X / spriteInfoPtr2->Source.Z : spriteInfoPtr2->Origin.X * 2E+32f;
                        float num7 = (double)spriteInfoPtr2->Source.W != 0.0 ? spriteInfoPtr2->Origin.Y / spriteInfoPtr2->Source.W : spriteInfoPtr2->Origin.Y * 2E+32f;
                        for (int index2 = 0; index2 < 4; ++index2)
                        {
                            float num8 = SpriteBatch.xCornerOffsets[index2];
                            float num9 = SpriteBatch.yCornerOffsets[index2];
                            float num10 = (num8 - num6) * spriteInfoPtr2->Destination.Z;
                            float num11 = (num9 - num7) * spriteInfoPtr2->Destination.W;
                            float num12 = (float)((double)spriteInfoPtr2->Destination.X + (double)num10 * (double)num4 - (double)num11 * (double)num5);
                            float num13 = (float)((double)spriteInfoPtr2->Destination.Y + (double)num10 * (double)num5 + (double)num11 * (double)num4);
                            if ((spriteInfoPtr2->Effects & SpriteEffects.FlipHorizontally) != SpriteEffects.None)
                                num8 = 1f - num8;
                            if ((spriteInfoPtr2->Effects & SpriteEffects.FlipVertically) != SpriteEffects.None)
                                num9 = 1f - num9;
                            positionColorTexturePtr2->Position.X = num12;
                            positionColorTexturePtr2->Position.Y = num13;
                            positionColorTexturePtr2->Position.Z = spriteInfoPtr2->Depth;
                            positionColorTexturePtr2->Color = spriteInfoPtr2->Color;
                            positionColorTexturePtr2->TextureCoordinate.X = (spriteInfoPtr2->Source.X + num8 * spriteInfoPtr2->Source.Z) * num1;
                            positionColorTexturePtr2->TextureCoordinate.Y = (spriteInfoPtr2->Source.Y + num9 * spriteInfoPtr2->Source.W) * num2;
                            ++positionColorTexturePtr2;
                        }
                        ++spriteInfoPtr2;
                    }
                }
                int vertexStride = sizeof(VertexPositionColorTexture);
                this.vertexBuffer.SetData<VertexPositionColorTexture>(this.vertexBufferPosition * vertexStride * 4, this.outputVertices, 0, num3 * 4, vertexStride, options);
                this._parent.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, this.vertexBufferPosition * 4, num3 * 4, this.vertexBufferPosition * 6, num3 * 2);
                this.vertexBufferPosition += num3;
                offset += num3;
                count -= num3;
            }
        }
        */

        //protected override void Dispose(bool disposing)
        //{
        //    try
        //    {
        //        if (!disposing || this.IsDisposed)
        //            return;
        //        if (this.spriteEffect != null)
        //            this.spriteEffect.Dispose();
        //        this.DisposePlatformData();
        //    }
        //    finally
        //    {
        //        base.Dispose(disposing);
        //    }
        //}

        public void Begin()
        {
            this.Begin(SpriteSortMode.Deferred, (BlendState)null, (SamplerState)null, (DepthStencilState)null, (RasterizerState)null, (Effect)null, Matrix.Identity);
        }

        public void Begin(SpriteSortMode sortMode, BlendState blendState)
        {
            this.Begin(sortMode, blendState, (SamplerState)null, (DepthStencilState)null, (RasterizerState)null, (Effect)null, Matrix.Identity);
        }

        public void Begin(SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState, RasterizerState rasterizerState)
        {
            this.Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, (Effect)null, Matrix.Identity);
        }

        public void Begin(SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState, RasterizerState rasterizerState, Effect effect)
        {
            this.Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect, Matrix.Identity);
        }

        public void Begin(SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState, RasterizerState rasterizerState, Effect effect, Matrix transformMatrix)
        {
            if (this.inBeginEndPair)
                throw new InvalidOperationException("End must be called before begin");
            this.spriteSortMode = sortMode;
            this.blendState = blendState;
            this.samplerState = samplerState;
            this.depthStencilState = depthStencilState;
            this.rasterizerState = rasterizerState;
            this.customEffect = effect;
            this.transformMatrix = transformMatrix;
            if (sortMode == SpriteSortMode.Immediate)
            {
                if (spriteBeginCount > 0)
                    throw new InvalidOperationException("Cannot Next Sprite Begin Immediate");
                SetRenderState();
                ++spriteImmediateBeginCount;
            }
            else if (spriteImmediateBeginCount > 0)
                throw new InvalidOperationException("Cannot Next Sprite Begin Immediate");
            ++spriteBeginCount;
            this.inBeginEndPair = true;
        }

        public void End()
        {
            if (!this.inBeginEndPair)
                throw new InvalidOperationException("Begin Must Be Called Before End");
            if (this.spriteSortMode != SpriteSortMode.Immediate)
                this.SetRenderState();
            else
                --spriteImmediateBeginCount;
            if (this.spriteQueueCount > 0)
                this.Flush();
            this.inBeginEndPair = false;
            --spriteBeginCount;
        }

        //public void Draw(Texture2D texture, Vector2 position, Color color)
        //{
        //    this.InternalDraw(texture, ref new Vector4()
        //    {
        //        X = position.X,
        //        Y = position.Y,
        //        Z = 1f,
        //        W = 1f
        //    }, true, ref SpriteBatch.nullRectangle, color, 0.0f, ref SpriteBatch.vector2Zero, SpriteEffects.None, 0.0f);
        //}

        //public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color)
        //{
        //    this.InternalDraw(texture, ref new Vector4()
        //    {
        //        X = position.X,
        //        Y = position.Y,
        //        Z = 1f,
        //        W = 1f
        //    }, true, ref sourceRectangle, color, 0.0f, ref SpriteBatch.vector2Zero, SpriteEffects.None, 0.0f);
        //}

        //public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
        //{
        //    this.InternalDraw(texture, ref new Vector4()
        //    {
        //        X = position.X,
        //        Y = position.Y,
        //        Z = scale,
        //        W = scale
        //    }, true, ref sourceRectangle, color, rotation, ref origin, effects, layerDepth);
        //}

        //public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
        //{
        //    this.InternalDraw(texture, ref new Vector4()
        //    {
        //        X = position.X,
        //        Y = position.Y,
        //        Z = scale.X,
        //        W = scale.Y
        //    }, true, ref sourceRectangle, color, rotation, ref origin, effects, layerDepth);
        //}

        //public void Draw(Texture2D texture, Rectangle destinationRectangle, Color color)
        //{
        //    this.InternalDraw(texture, ref new Vector4()
        //    {
        //        X = (float)destinationRectangle.X,
        //        Y = (float)destinationRectangle.Y,
        //        Z = (float)destinationRectangle.Width,
        //        W = (float)destinationRectangle.Height
        //    }, false, ref SpriteBatch.nullRectangle, color, 0.0f, ref SpriteBatch.vector2Zero, SpriteEffects.None, 0.0f);
        //}

        //public void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color)
        //{
        //    this.InternalDraw(texture, ref new Vector4()
        //    {
        //        X = (float)destinationRectangle.X,
        //        Y = (float)destinationRectangle.Y,
        //        Z = (float)destinationRectangle.Width,
        //        W = (float)destinationRectangle.Height
        //    }, false, ref sourceRectangle, color, 0.0f, ref SpriteBatch.vector2Zero, SpriteEffects.None, 0.0f);
        //}

        //public void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effects, float layerDepth)
        //{
        //    this.InternalDraw(texture, ref new Vector4()
        //    {
        //        X = (float)destinationRectangle.X,
        //        Y = (float)destinationRectangle.Y,
        //        Z = (float)destinationRectangle.Width,
        //        W = (float)destinationRectangle.Height
        //    }, false, ref sourceRectangle, color, rotation, ref origin, effects, layerDepth);
        //}

        //public void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color)
        //{
        //    if (spriteFont == null)
        //        throw new ArgumentNullException("spriteFont");
        //    if (text == null)
        //        throw new ArgumentNullException("text");
        //    SpriteFont.StringProxy text1 = new SpriteFont.StringProxy(text);
        //    Vector2 one = Vector2.One;
        //    spriteFont.InternalDraw(ref text1, this, position, color, 0.0f, Vector2.Zero, ref one, SpriteEffects.None, 0.0f);
        //}

        //public void DrawString(SpriteFont spriteFont, StringBuilder text, Vector2 position, Color color)
        //{
        //    if (spriteFont == null)
        //        throw new ArgumentNullException("spriteFont");
        //    if (text == null)
        //        throw new ArgumentNullException("text");
        //    SpriteFont.StringProxy text1 = new SpriteFont.StringProxy(text);
        //    Vector2 one = Vector2.One;
        //    spriteFont.InternalDraw(ref text1, this, position, color, 0.0f, Vector2.Zero, ref one, SpriteEffects.None, 0.0f);
        //}

        //public void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
        //{
        //    if (spriteFont == null)
        //        throw new ArgumentNullException("spriteFont");
        //    if (text == null)
        //        throw new ArgumentNullException("text");
        //    SpriteFont.StringProxy text1 = new SpriteFont.StringProxy(text);
        //    spriteFont.InternalDraw(ref text1, this, position, color, rotation, origin, ref new Vector2()
        //    {
        //        X = scale,
        //        Y = scale
        //    }, effects, layerDepth);
        //}

        //public void DrawString(SpriteFont spriteFont, StringBuilder text, Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
        //{
        //    if (spriteFont == null)
        //        throw new ArgumentNullException("spriteFont");
        //    if (text == null)
        //        throw new ArgumentNullException("text");
        //    SpriteFont.StringProxy text1 = new SpriteFont.StringProxy(text);
        //    spriteFont.InternalDraw(ref text1, this, position, color, rotation, origin, ref new Vector2()
        //    {
        //        X = scale,
        //        Y = scale
        //    }, effects, layerDepth);
        //}

        //public void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
        //{
        //    if (spriteFont == null)
        //        throw new ArgumentNullException("spriteFont");
        //    if (text == null)
        //        throw new ArgumentNullException("text");
        //    SpriteFont.StringProxy text1 = new SpriteFont.StringProxy(text);
        //    spriteFont.InternalDraw(ref text1, this, position, color, rotation, origin, ref scale, effects, layerDepth);
        //}

        //public void DrawString(SpriteFont spriteFont, StringBuilder text, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
        //{
        //    if (spriteFont == null)
        //        throw new ArgumentNullException("spriteFont");
        //    if (text == null)
        //        throw new ArgumentNullException("text");
        //    SpriteFont.StringProxy text1 = new SpriteFont.StringProxy(text);
        //    spriteFont.InternalDraw(ref text1, this, position, color, rotation, origin, ref scale, effects, layerDepth);
        //}

        //TODO: fix internal
        
        private void InternalDraw(Texture2D texture, ref Vector4 destination, bool scaleDestination, ref Rectangle? sourceRectangle, Color color, float rotation, ref Vector2 origin, SpriteEffects effects, float depth)
        {
            if (texture == null)
                throw new ArgumentNullException("texture is null");
            if (!this.inBeginEndPair)
                throw new InvalidOperationException("Begin Must Be Called Before Draw");
            if (this.spriteQueueCount >= this.spriteQueue.Length)
                Array.Resize<DrawBatch.SpriteInfo>(ref this.spriteQueue, this.spriteQueue.Length * 2);
            DrawBatch.SpriteInfo spriteInfoPtr = this.spriteQueue[this.spriteQueueCount])
            float num1 = destination.Z;
                float num2 = destination.W;
                if (sourceRectangle.HasValue)
                {
                    Rectangle rectangle = sourceRectangle.Value;
                    spriteInfoPtr.Source.X = (float)rectangle.X;
                    spriteInfoPtr.Source.Y = (float)rectangle.Y;
                    spriteInfoPtr.Source.Z = (float)rectangle.Width;
                    spriteInfoPtr.Source.W = (float)rectangle.Height;
                    if (scaleDestination)
                    {
                        num1 *= (float)rectangle.Width;
                        num2 *= (float)rectangle.Height;
                    }
                }
                else
                {
                    float num3 = (float)texture.Width;
                    float num4 = (float)texture.Height;
                    spriteInfoPtr.Source.X = 0.0f;
                    spriteInfoPtr.Source.Y = 0.0f;
                    spriteInfoPtr.Source.Z = num3;
                    spriteInfoPtr.Source.W = num4;
                    if (scaleDestination)
                    {
                        num1 *= num3;
                        num2 *= num4;
                    }
                }
                spriteInfoPtr.Destination.X = destination.X;
                spriteInfoPtr.Destination.Y = destination.Y;
                spriteInfoPtr.Destination.Z = num1;
                spriteInfoPtr.Destination.W = num2;
                spriteInfoPtr.Origin.X = origin.X;
                spriteInfoPtr.Origin.Y = origin.Y;
                spriteInfoPtr.Rotation = rotation;
                spriteInfoPtr.Depth = depth;
                spriteInfoPtr.Effects = effects;
                spriteInfoPtr.Color = color;
            
            if (this.spriteSortMode == SpriteSortMode.Immediate)
            {
                this.RenderBatch(texture, this.spriteQueue, 0, 1);
            }
            else
            {
                if (this.spriteTextures == null || this.spriteTextures.Length != this.spriteQueue.Length)
                    Array.Resize<Texture2D>(ref this.spriteTextures, this.spriteQueue.Length);
                this.spriteTextures[this.spriteQueueCount] = texture;
                ++this.spriteQueueCount;
            }
        }

        private void Flush()
        {
            DrawBatch.SpriteInfo[] sprites;
            if (this.spriteSortMode == SpriteSortMode.Deferred)
            {
                sprites = this.spriteQueue;
            }
            else
            {
                this.SortSprites();
                sprites = this.sortedSprites;
            }
            int offset = 0;
            Texture2D texture = (Texture2D)null;
            for (int index1 = 0; index1 < this.spriteQueueCount; ++index1)
            {
                Texture2D texture2D;
                if (this.spriteSortMode == SpriteSortMode.Deferred)
                {
                    texture2D = this.spriteTextures[index1];
                }
                else
                {
                    int index2 = this.sortIndices[index1];
                    sprites[index1] = this.spriteQueue[index2];
                    texture2D = this.spriteTextures[index2];
                }
                if (texture2D != texture)
                {
                    if (index1 > offset)
                        this.RenderBatch(texture, sprites, offset, index1 - offset);
                    offset = index1;
                    texture = texture2D;
                }
            }
            this.RenderBatch(texture, sprites, offset, this.spriteQueueCount - offset);
            Array.Clear((Array)this.spriteTextures, 0, this.spriteQueueCount);
            this.spriteQueueCount = 0;
        }

        private void SortSprites()
        {
            if (this.sortIndices == null || this.sortIndices.Length < this.spriteQueueCount)
            {
                this.sortIndices = new int[this.spriteQueueCount];
                this.sortedSprites = new DrawBatch.SpriteInfo[this.spriteQueueCount];
            }
            IComparer<int> comparer;
            switch (this.spriteSortMode)
            {
                case SpriteSortMode.Texture:
                    if (this.textureComparer == null)
                        this.textureComparer = new DrawBatch.TextureComparer(this);
                    comparer = (IComparer<int>)this.textureComparer;
                    break;
                case SpriteSortMode.BackToFront:
                    if (this.backToFrontComparer == null)
                        this.backToFrontComparer = new DrawBatch.BackToFrontComparer(this);
                    comparer = (IComparer<int>)this.backToFrontComparer;
                    break;
                case SpriteSortMode.FrontToBack:
                    if (this.frontToBackComparer == null)
                        this.frontToBackComparer = new DrawBatch.FrontToBackComparer(this);
                    comparer = (IComparer<int>)this.frontToBackComparer;
                    break;
                default:
                    throw new NotSupportedException();
            }
            for (int index = 0; index < this.spriteQueueCount; ++index)
                this.sortIndices[index] = index;
            Array.Sort<int>(this.sortIndices, 0, this.spriteQueueCount, comparer);
        }

        private void RenderBatch(Texture2D texture, DrawBatch.SpriteInfo[] sprites, int offset, int count)
        {
            if (this.customEffect != null)
            {
                int count1 = this.customEffect.CurrentTechnique.Passes.Count;
                for (int index = 0; index < count1; ++index)
                {
                    this.customEffect.CurrentTechnique.Passes[index].Apply();
                    graphicsDevice.Textures[0] = (Texture)texture;
                    this.PlatformRenderBatch(texture, sprites, offset, count);
                }
            }
            else
            {
                graphicsDevice.Textures[0] = (Texture)texture;
                this.PlatformRenderBatch(texture, sprites, offset, count);
            }
        }

        private void SetRenderState()
        {
            if (this.blendState != null)
                graphicsDevice.BlendState = this.blendState;
            else
                graphicsDevice.BlendState = BlendState.AlphaBlend;
            if (this.depthStencilState != null)
                graphicsDevice.DepthStencilState = this.depthStencilState;
            else
                graphicsDevice.DepthStencilState = DepthStencilState.None;
            if (this.rasterizerState != null)
                graphicsDevice.RasterizerState = this.rasterizerState;
            else
                graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            if (this.samplerState != null)
                graphicsDevice.SamplerStates[0] = this.samplerState;
            else
                graphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
            Viewport viewport = graphicsDevice.Viewport;
            float num1 = viewport.Width > 0 ? 1f / (float)viewport.Width : 0.0f;
            float num2 = viewport.Height > 0 ? -1f / (float)viewport.Height : 0.0f;
            Matrix matrix = new Matrix();
            matrix.M11 = num1 * 2f;
            matrix.M22 = num2 * 2f;
            matrix.M33 = 1f;
            matrix.M44 = 1f;
            matrix.M41 = -1f;
            matrix.M42 = 1f;
            matrix.M41 -= num1;
            matrix.M42 -= num2;
            this.effectMatrixTransform.SetValue(this.transformMatrix * matrix);
            this.spriteEffect.CurrentTechnique.Passes[0].Apply();
            this.SetPlatformRenderState();
        }

        private struct SpriteInfo
        {
            public Vector4 Source;
            public Vector4 Destination;
            public Vector2 Origin;
            public float Rotation;
            public float Depth;
            public SpriteEffects Effects;
            public Color Color;
        }

        private class TextureComparer : IComparer<int>
        {
            private DrawBatch parent;

            public TextureComparer(DrawBatch parent)
            {
                this.parent = parent;
            }

            public int Compare(int x, int y)
            {
                //return this.parent.spriteTextures[x].CompareTo((Texture)this.parent.spriteTextures[y]);
                return 0;
            }
        }

        private class BackToFrontComparer : IComparer<int>
        {
            private DrawBatch parent;

            public BackToFrontComparer(DrawBatch parent)
            {
                this.parent = parent;
            }

            public int Compare(int x, int y)
            {
                float num1 = this.parent.spriteQueue[x].Depth;
                float num2 = this.parent.spriteQueue[y].Depth;
                if ((double)num1 > (double)num2)
                    return -1;
                return (double)num1 < (double)num2 ? 1 : 0;
            }
        }

        private class FrontToBackComparer : IComparer<int>
        {
            private DrawBatch parent;

            public FrontToBackComparer(DrawBatch parent)
            {
                this.parent = parent;
            }

            public int Compare(int x, int y)
            {
                float num1 = parent.spriteQueue[x].Depth;
                float num2 = this.parent.spriteQueue[y].Depth;
                if ((double)num1 > (double)num2)
                    return 1;
                return (double)num1 < (double)num2 ? -1 : 0;
            }
        }
    }
}
