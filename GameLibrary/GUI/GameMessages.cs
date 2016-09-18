using System;
using System.Linq;
using Engine.Core;
using Engine.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace GameLibrary.Gui
{
    public class InfoRecord
    {
        public string Prefix;

        public string Value; 

        public bool Visible;

        public TextRegion Region;
    }

    public class GameMessages : IGraphicsObject
    {
        public const int Maxvaluelenght = 8;

        public const string Separator = ": ";

        public readonly Dictionary<string, InfoRecord> Records;

        private readonly SpriteFont _font;
        

        public GameMessages(SpriteFont font)
        {
            _font = font;
            Records = new Dictionary<string, InfoRecord>();
            StaticPosition = true;
        }

        public void RegisterRecord(string key, string prefix, string value, bool visible)
        {
            Records.Add(key, new InfoRecord
                                 {
                                     Prefix = prefix,
                                     Value = value,
                                     Visible = visible
                                 });
        }

        public int MaxLenght()
        {
            int max = Records.Select(record => (record.Value.Prefix.Length + Maxvaluelenght)).Concat(new[] {0}).Max();

            return max*10+5;
        }

        public void InitInfoRegion()
        {
            var maxHeight = Records.Count*5+10;
            var maxLenght = MaxLenght();
            int i = 0;

            if((maxLenght < 400) && (maxHeight <= 480))
            {
                
                foreach (var record in Records)
                {
                    record.Value.Region = new TextRegion(
                        new Rectangle(
                            800 - maxLenght,
                            15*i + 5,
                            maxLenght,
                            maxHeight
                            ),
                        _font,
                        Color.LimeGreen,
                        record.Value.Prefix + Separator + record.Value.Value,
                        true);
                    i++;
                }
            }
        }

        public void UpdateInfoText(string key, string text)
        {
            Records[key].Region.Text = Records[key].Prefix + Separator + text;
        }

        public bool StaticPosition { get; set; }
        public bool IgnoreCulling { get { return true; } set { throw new NotImplementedException();} }
        public Rectangle Rect { get { throw new NotImplementedException();} set { throw new NotImplementedException();} }
        [ContentSerializerIgnore]
        public Rectangle CornerRectangle { get { return Rect; } }
        public event SimpleEvent OnPositionTypeChanged;
        public void Draw()
        {
            foreach (var record in Records)
            {
                if (record.Value.Visible)
                {
                    record.Value.Region.Draw();
                }
            }
        }
    }
}