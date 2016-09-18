using System;
using System.Collections.Generic;
using Engine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Graphics
{
    public class TextRegion : GameObject
    {
        public FontHorizontalAlign HorizontaAlign = FontHorizontalAlign.Left;
        public FontVerticalAlign VerticalAlign = FontVerticalAlign.Top;
        public SpriteFont Font { get; set; }

        protected int _maxWidth;
        //protected int _maxHeight;
        protected int _textStart;
        protected int _textLength;
        protected string _text;
        protected string _textToDraw;
        protected List<string> _lines;
        public bool Multiline { get; protected set; }
        protected bool _wordWrap;
        protected int _startPosition;

        public int StartPosition
        {
            get { return _startPosition; }
            set
            {
                if (_startPosition == value) return;
                _startPosition = value;
                UpdateText();
            }
        }

        

        /// <summary>
        /// Text to draw
        /// </summary>
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                UpdateText();
            }
        }
#if EDITOR
        public TextRegion(Rectangle rect, SpriteFont font, string text = "", bool multiline = false)
        {
            Init(rect, text, Color.White, font, multiline);
        }

        public TextRegion(Rectangle rect, SpriteFont font, Color color, string text, bool multiline)
        {
            Init(rect, text, color, font, multiline);
        }
#else
        public TextRegion(Rectangle rect, SpriteFont font, string text, bool multiline)
        {
            Init(rect, text, Color.White, font, multiline);
        }

        public TextRegion(Rectangle rect, SpriteFont font, Color color, string text, bool multiline)
        {
            Init(rect, text, color, font, multiline);
        }
#endif

        protected void Init(Rectangle rect, string text, Color color, SpriteFont font, bool multiline)
        {
            Font = font;
            Color = color;
            _rect = rect;
            CornerRectangle = _rect;
            _text = text;
            Multiline = multiline;
            StaticPosition = true;
            UpdateText();
            // _textToDraw = "";
        }

        protected override void SetDrawRectangle()
        {
            base.SetDrawRectangle();
            if (!string.IsNullOrEmpty(_text))
                UpdateText();
        }

        public Vector2 GetSize()
        {
            return Font.MeasureString(_textToDraw);
        }

        protected void UpdateText()
        {
            if (Font == null)
                return;
            
            if (!Multiline)
            {
                _textToDraw = _text.Replace("\n", "");
                if (StartPosition != 0)
                {
                    if (StartPosition >= _text.Length)
                    {
                        _textToDraw = "";
                        return;
                    }
                    _textToDraw = _textToDraw.Substring(StartPosition);
                }

                if (Font.MeasureString(_textToDraw).X > Rect.Width)
                {
                    var words = _textToDraw.Split(new[] {' '});
                    _textToDraw = "";
                    for (var i = 0; i < words.Length; i++)
                    {
                        var word = words[i];
                        if (i < words.Length - 1)
                            word += " ";
                        if (Font.MeasureString(_textToDraw + word).X > Rect.Width)
                        {
                            foreach (var c in word)
                            {
                                if (Font.MeasureString(_textToDraw + c).X > Rect.Width)
                                    break;
                                _textToDraw += c;
                            }
                            break;
                        }
                        _textToDraw += word;
                    }
                }
            }
            else
            {


                _textToDraw = _text.Replace("\\n", "\n");
                if (StartPosition != 0)
                {
                    if (StartPosition >= _text.Length)
                    {
                        _textToDraw = "";
                        return;
                    }
                    _textToDraw = _textToDraw.Substring(StartPosition);
                }
                var words = _textToDraw.SplitAndKeep(new[] {'\n', ' ', ',', '.', ';'});
                var lineBreak = true;
                var line = "";
                var height = 0;
                _textToDraw = "";
                for (var i = 0; i < words.Length; i++)
                {
                    var word = words[i];
                    if (word == "\n")
                    {
                        var lineHeight = (int) Font.MeasureString(line).Y;
                        height += lineHeight;
                        lineBreak = true;
                        if (height + lineHeight > Rect.Height)
                            break;
                        _textToDraw += line + "\n";
                        line = "";
                        continue;
                    }
                    if (Font.MeasureString(line + word).X > Rect.Width)
                    {
                        if (lineBreak || Font.MeasureString(word).X > (Rect.Width / 2))
                        {
                            foreach (var c in word)
                            {
                                if (Font.MeasureString(line + c).X > Rect.Width)
                                {
                                    var lineHeight = (int) Font.MeasureString(line).Y;
                                    if (height + lineHeight > Rect.Height) // line takes more height than rect.height - delete line and cancel
                                    {
                                        line = "";
                                        break;
                                    }
                                    height += lineHeight;
                                    _textToDraw += line + "\n";
                                    line = "";
                                }
                                line += c;
                            }
                            if (line == "")
                            {
                                if (height + (int) Font.MeasureString(word).Y > Rect.Height)
                                    break;
                                lineBreak = true;
                            }

                        }
                        else
                        {
                            if (word != " ")
                                i--;

                            line += "\n";
                            lineBreak = true;
                            _textToDraw += line;
                            height += (int) Font.MeasureString(line).Y;
                            if (height + (int) Font.MeasureString(line).Y > Rect.Height)
                                break;
                            line = "";
                        }
                    }
                    else
                    {
                        line += word;
                        lineBreak = false;
                    }
                }
                _textToDraw += line;
            }

        }

        public override bool IgnoreCulling { get { return this._staticPosition; } set { throw new NotImplementedException();} }

        public override void Draw()
        {

            if (IsHidden)
                return;
            var viewPoint = new Vector2(_rect.X, _rect.Y);

            var pos = new Vector2();
            switch (HorizontaAlign)
            {
                case FontHorizontalAlign.Left:
                    pos.X = viewPoint.X;
                    
                    break;
                case FontHorizontalAlign.Center:
                    pos.X = viewPoint.X + (_rect.Width / 2f) - (Font.MeasureString(_textToDraw).X * _scale.X / 2f);

                    break;
                case FontHorizontalAlign.Right:
                    pos.X = viewPoint.X + _rect.Width - (Font.MeasureString(_textToDraw).X * _scale.X  / 2f);
                    break;
            }
            switch (VerticalAlign)
            {
                case FontVerticalAlign.Top:
                    pos.Y = viewPoint.Y;
                    break;
                case FontVerticalAlign.Center:
                    pos.Y = viewPoint.Y + (_rect.Height/2f) - (Font.MeasureString(_textToDraw).Y * _scale.Y/2f);
                    break;
                case FontVerticalAlign.Bottom:
                    pos.Y = viewPoint.Y + _rect.Height - (Font.MeasureString(_textToDraw).Y * _scale.Y);
                    break;
            }

            EngineGlobals.Batch.DrawString(Font, _textToDraw, pos, Color, Rotation,
                                                   new Vector2(0, 0), Scale, SpriteEffects.None, _layerDepth);
        }


        //protected override Rectangle CalculateCornerRectangle()
        //{
        //    return _rect;
        //}
    }
}
