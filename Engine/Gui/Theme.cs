using Engine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Gui
{
    public class Theme
    {
        [ContentSerializerIgnore]
        public SpriteFont Font;

        [ContentSerializerIgnore]
        public Texture2D Texutre;

        protected string _fontName;
        protected string _textureFile;

        [ContentSerializer(ElementName = "Font")]
        public string FontName
        {
            set
            {
                _fontName = value;
                if (EngineGlobals.ContentCache != null && value != "")
                    Font = EngineGlobals.ContentCache.Load<SpriteFont>(value);
                
            }
            get { return _fontName; }
        }

        [ContentSerializer(ElementName = "Texture")]
        public string TextureFile
        {
            set
            {
                _textureFile = value;
                if (EngineGlobals.ContentCache != null && value != "")
                    Texutre = EngineGlobals.ContentCache.Load<Texture2D>(value);
            }
            get { return _textureFile; }
        }
        public Rectangle WindowBackground;
        public Rectangle EditBackground;
    }
}
