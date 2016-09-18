using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using Engine.Core;
using Engine.Graphics;
using Engine.Mechanics;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace GameEditor.Sprite_Editor
{
    public partial class SpriteControl : UserControl
    {
        [Description("SpriteRoot")]
        [TypeConverter(typeof(PropertySorter))]
        public class SpriteRootController : INotifyPropertyChanging, INotifyPropertyChanged
        {
            protected Size _drawSize;
            protected SpriteControl _owner;

            public Sprite Sprite;

            [PropertyOrder(18)]
            public Size DrawSize
            {
                get { return _drawSize; }
                set
                {
                    ExecutePropertyChanging("DrawSize");
                    _drawSize = value;
                    Sprite.Rect = new Rectangle((int)(_owner.spriteWindow.Width/2f - _drawSize.Width/2f),
                                                 (int)(_owner.spriteWindow.Height/2f - _drawSize.Height/2f), _drawSize.Width,
                                                 _drawSize.Height);
                    ExecutePropertyChaned("DrawSize");
                }
            }

            [PropertyOrder(6)]
            public string Version { get { return Sprite.Data.Version.ToString("#0.0"); } }

            [PropertyOrder(12)]
            public int Animations { get { return Sprite.Data.Animations.Length; } }

            public SpriteRootController(SpriteControl owner, Sprite sprite)
            {
                Sprite = sprite;
                _drawSize = new Size(Sprite.Rect.Width, Sprite.Rect.Height);
                _owner = owner;
            }

            public void ExecutePropertyChanging(string name)
            {
                if (PropertyChanging != null)
                    PropertyChanging(this, new PropertyChangingEventArgs(name));
            }

            public void ExecutePropertyChaned(string name)
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(name));
            }

            public event PropertyChangingEventHandler PropertyChanging;
            public event PropertyChangedEventHandler PropertyChanged;

            public override string ToString()
            {
                return "Sprite Root";
            }
        }

        public SpriteRootController Root;

        public SpriteControl()
        {
            InitializeComponent();
            spriteWindow.Invalidated += SpriteWindowOnInvalidated;
        }

        private void SpriteWindowOnInvalidated(object sender, InvalidateEventArgs invalidateEventArgs)
        {
            if (Stop.Enabled)
            {
                if (TrackBar.Value != spriteWindow.Sprite.CurrentFrame)
                    TrackBar.Value = spriteWindow.Sprite.CurrentFrame;
            }
        }

        public void NewSprite()
        {
            if (spriteWindow.Sprite != null)
                spriteWindow.Sprite.Dispose();
            comboBox1.Items.Clear();
            var data = new SpriteData { Animations = new Animation[0] };
            spriteWindow.Sprite = new Sprite(data, new Rectangle(0, 0, spriteWindow.Width, spriteWindow.Height));
            Root = new SpriteRootController(this, spriteWindow.Sprite);
            comboBox1.Items.Add(Root);
            comboBox1.SelectedItem = Root;
            spriteWindow.Sprite.AnimationFinished += SpriteOnAnimationFinished;
            spriteWindow.Sprite.AnimationPropertyChanged += SpriteOnAnimationPropertyChanged;
        }

        public void LoadSprite(string fname)
        {
            if (spriteWindow.Sprite != null)
                spriteWindow.Sprite.Dispose();
            comboBox1.Items.Clear();
            SpriteData data = null;
            using (var reader = XmlReader.Create(fname))
            {
                data = IntermediateSerializer.Deserialize<SpriteData>(reader, null);
            }
            spriteWindow.Sprite = new Sprite(data, new Rectangle(0,0, spriteWindow.Width, spriteWindow.Height));
            Root = new SpriteRootController(this, spriteWindow.Sprite);
            comboBox1.Items.Add(Root);
            foreach (var animation in data.Animations)
            {
                comboBox1.Items.Add(animation);
            }
            comboBox1.SelectedItem = Root;
            spriteWindow.Sprite.AnimationFinished += SpriteOnAnimationFinished;
            spriteWindow.Sprite.AnimationPropertyChanged += SpriteOnAnimationPropertyChanged;
        }

        private void SpriteOnAnimationPropertyChanged(object sender, string propertyName, object valueBeforeChange, object valueAfterChange)
        {
            if (propertyName == "Name")
            {
                var file = Directory.GetFiles(SpriteEditor.Window.AnimationsPath, (string)valueBeforeChange + ".*").First();
                var contentFile = Path.Combine(SpriteEditor.Window.ContentPath, SpriteEditor.Window.SpriteName, "Animations", (string)valueBeforeChange + ".xnb");
                File.Move(file, Path.Combine(Path.GetDirectoryName(file), (string)valueAfterChange + Path.GetExtension(file)));
                File.Move(contentFile, Path.Combine(Path.GetDirectoryName(contentFile), (string)valueAfterChange + ".xnb"));
                spriteWindow.Sprite.CurrentAnimation.SetPathPassive(Path.GetDirectoryName(spriteWindow.Sprite.CurrentAnimation.Path) + "\\" + (string)valueAfterChange);
                SpriteEditor.Window.Save();
            }
            else
            {
                if (propertyName == "Frames")
                {
                    TrackBar.Maximum = (int) valueAfterChange-1;
                    TrackBar.Enabled = TrackBar.Maximum > 1;
                }
                SpriteEditor.Window.Saved = false;
            }
        }

        private void SpriteOnAnimationFinished(object sender)
        {
            StopClick(this, new EventArgs());
        }

        public void AddAnimation(Animation animation)
        {
            Root.Sprite.AddAnimation(animation);
            comboBox1.Items.Add(animation);
            comboBox1.SelectedItem = animation;
        }

        private void ComboBox1SelectedIndexChanged(object sender, System.EventArgs e)
        {
            propertyGrid.SelectedObject = comboBox1.SelectedItem;
            if (propertyGrid.SelectedObject is Animation)
            {
                var animation = (Animation) propertyGrid.SelectedObject;
                spriteWindow.Sprite.SetAnimation(animation.Name);
                spriteWindow.SelectedAnimation = animation.Name;
                Play.Enabled = true;
                
                Stop.Enabled = false;
                Delete.Enabled = true;
                IsLoopedBox.Enabled = true;
                TrackBar.Value = 0;
                TrackBar.Maximum = animation.Frames-1;
                TrackBar.LargeChange = 1;
                TrackBar.SmallChange = 1;
                TrackBar.Enabled = TrackBar.Maximum > 1;
            }
            else
            {
                Play.Enabled = false;
                TrackBar.Enabled = false;
                Stop.Enabled = false;
                Delete.Enabled = false;
                IsLoopedBox.Enabled = false;
                spriteWindow.SelectedAnimation = null;
            }
        }

        private void TrackBarScroll(object sender, EventArgs e)
        {
            spriteWindow.Sprite.CurrentFrame = TrackBar.Value;
        }

        private void DeleteClick(object sender, EventArgs e)
        {
            var curAnim = spriteWindow.Sprite.CurrentFrame;
            if (MessageBox.Show("Delete animation '" + spriteWindow.Sprite.Data.Animations[curAnim].Name + "'?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;
            var file = Directory.GetFiles(SpriteEditor.Window.AnimationsPath,
                               spriteWindow.Sprite.Data.Animations[curAnim].Name + ".*").First();
            var contentFile = Path.Combine(SpriteEditor.Window.ContentPath, SpriteEditor.Window.SpriteName, "Animations", spriteWindow.Sprite.Data.Animations[curAnim].Name + ".xnb");
            File.Delete(file);
            File.Delete(contentFile);
            //File.Delete(SpriteEditor.Window.ContentPath);
            comboBox1.SelectedItem = Root;
            comboBox1.Items.Remove(spriteWindow.Sprite.Data.Animations[curAnim]);
            spriteWindow.Sprite.RemoveAnimation(curAnim);
            propertyGrid.Refresh();
            SpriteEditor.Window.Save();
        }

        private void PlayClick(object sender, EventArgs e)
        {
            comboBox1.Enabled = false;
            propertyGrid.Enabled = false;
            TrackBar.Enabled = false;
            IsLoopedBox.Enabled = false;
            Play.Enabled = false;
            Stop.Enabled = true;
            Delete.Enabled = false;
            spriteWindow.Sprite.ResumeAnimation(IsLoopedBox.Checked);
            SpriteEditor.Window.toolStrip1.Enabled = false;
        }

        private void StopClick(object sender, EventArgs e)
        {
            comboBox1.Enabled = true;
            propertyGrid.Enabled = true;
            TrackBar.Enabled = TrackBar.Maximum > 1;
            IsLoopedBox.Enabled = true;
            Play.Enabled = true;
            Stop.Enabled = false;
            Delete.Enabled = true;
            spriteWindow.Sprite.StopAnimation();
            SpriteEditor.Window.toolStrip1.Enabled = true;
            TrackBar.Value = 0;
            spriteWindow.Sprite.CurrentFrame = 0;
        }

        public void CloseSprite()
        {
            comboBox1.Items.Clear();
            if (spriteWindow.Sprite == null)
                return;
            spriteWindow.Sprite.Dispose();
            spriteWindow.Sprite = null;
            Root = null;
            Play.Enabled = false;
            IsLoopedBox.Enabled = false;
            TrackBar.Enabled = false;
            Delete.Enabled = false;
            propertyGrid.SelectedObject = null;
        }
    }
}
