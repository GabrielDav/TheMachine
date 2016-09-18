using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using Engine.Core;
using Engine.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using XnaContentCompiler;

namespace GameEditor.Sprite_Editor
{


    public partial class SpriteEditor : Form
    {
        public static SpriteEditor Window;
        public SpriteControl ActiveSpriteWindow;
        public ContentBuilder ContentBuilder;
        protected string _filename;
        public readonly string RootPath;
        public readonly string ContentPath;
        public string AnimationsPath;
        public string SpritePath;
        public string SpriteName;
        public readonly string TempPath;
        public ContentManager Content;

        public bool Saved
        {
            get { return !toolStripSaveButton.Enabled; }
            set { toolStripSaveButton.Enabled = !value; }
        }

        public SpriteEditor()
        {
            InitializeComponent();
            Window = this;
            RootPath = Form1.CurrentPath + "\\Sprites\\";
            var path = Path.GetTempPath();
            TempPath = Path.Combine(path, "TheGoo_SpriteEditor");
            if (!Directory.Exists(TempPath))
                Directory.CreateDirectory(TempPath);
            ContentPath = Path.Combine(Form1.CurrentPath, "SpritesContent");
            if (!Directory.Exists(ContentPath))
                Directory.CreateDirectory(ContentPath);

        }

        private void SpriteEditorLoad(object sender, EventArgs e)
        {

            ContentBuilder = new ContentBuilder();

            if (!Directory.Exists(RootPath))
                Directory.CreateDirectory(RootPath);
            openFileDialog1.InitialDirectory = RootPath;
            var page = tabControl1.AddTab();
            ActiveSpriteWindow = (SpriteControl) page.Controls[0];
            Content = new ContentManager(ActiveSpriteWindow.spriteWindow.Services) {RootDirectory = "SpritesContent"};
        }

        private void ToolStripBackgroundColorPickerButtonClick(object sender, EventArgs e)
        {
            colorDialog1.Color = SpriteEditorSettings.Default.BackgroundColor;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                SpriteEditorSettings.Default.BackgroundColor = colorDialog1.Color;
                SpriteEditorSettings.Default.Save();
                ActiveSpriteWindow.spriteWindow.BackColor = colorDialog1.Color;
            }
        }

        public void Save()
        {
            var settings = new XmlWriterSettings {Indent = true};
            //var tempSpriteRoot = Path.Combine(_tempPath, _spriteName);
            //Directory.CreateDirectory(tempSpriteRoot);
            //var tempXmlPath = Path.Combine(tempSpriteRoot, _filename);
            using (var writer = XmlWriter.Create(Path.Combine(SpritePath, _filename), settings))
            {
                IntermediateSerializer.Serialize(writer, ActiveSpriteWindow.Root.Sprite.Data, null);
            }
            Saved = true;
            //ContentBuilder.Add(tempXmlPath, _spriteName);
            //ContentBuilder.Build(tempSpriteRoot);
            //ContentBuilder.Clear();
            //File.Delete(tempXmlPath);
            //File.Copy(Path.Combine(tempSpriteRoot, "content", _spriteName + ".xnb"), Path.Combine(_spritePath, _spriteName + ".xnb"), true);
        }

        private void ToolStripNewButtonClick(object sender, EventArgs e)
        {
            var dialog = new NewSpriteDialog();
            showdialog:
            var animationsPath = AnimationsPath;
            var spritePath = SpritePath;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                spritePath = Path.Combine(RootPath, dialog.textBox1.Text);
                animationsPath = Path.Combine(spritePath, "Animations");
                if (Directory.Exists(spritePath))
                {
                    if (
                        MessageBox.Show("Sprite named '" + dialog.textBox1.Text + "' already exists. Overtwrite?",
                                        "Warning", MessageBoxButtons.YesNo) == DialogResult.No)
                    {
                        goto showdialog;
                    }
                    try
                    {

                        var files = Directory.GetFiles(animationsPath);
                        foreach (var file in files)
                        {
                            File.Delete(file);
                        }
                        Directory.Delete(animationsPath);
                        var contentDir = Path.Combine(ContentPath, SpriteName, "Animations");
                        files = Directory.GetFiles(contentDir);
                        foreach (var file in files)
                        {
                            File.Delete(file);
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Unable to delete animations of sprite '" + dialog.textBox1.Text + "'.", "Error",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                        throw;
                    }

                }
                SpriteName = dialog.textBox1.Text;
                _filename = SpriteName + ".xml";
                SpritePath = spritePath;
                AnimationsPath = animationsPath;
                Directory.CreateDirectory(SpritePath);
                Directory.CreateDirectory(AnimationsPath);
                Directory.CreateDirectory(Path.Combine(ContentPath, SpriteName));
                Directory.CreateDirectory(Path.Combine(ContentPath, SpriteName, "Animations"));
                ActiveSpriteWindow.NewSprite();
                Save();
                toolStripImportAnimationButton.Enabled = true;
                toolStripExportButton.Enabled = true;
                toolStripDeleteSpriteButton.Enabled = true;
            }
            dialog.Dispose();
        }

        private void ToolStripImportButtonClick(object sender, EventArgs e)
        {
            var dialog = new ImportAnimationForm(ActiveSpriteWindow.spriteWindow.Sprite, this);
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var path = Path.Combine(SpriteName, "Animations", dialog.textBox2.Text);
                var editorContent = EngineGlobals.ContentCache;
                EngineGlobals.ContentCache = Content;
                var animation = new Animation
                    {
                        Path = path,
                        Name = dialog.textBox2.Text,
                        Frames = dialog.Frames,
                        FrameWidth = dialog.FrameWidth,
                        FrameHeight = dialog.FrameHeight,
                        Speed = 500
                    };
                ActiveSpriteWindow.AddAnimation(animation);
                Save();
                EngineGlobals.ContentCache = editorContent;
            }
            dialog.Dispose();
        }

        private void SpriteEditorFormClosing(object sender, FormClosingEventArgs e)
        {
            if (!Saved)
            {
                var result = MessageBox.Show("Sprite data not saved do You want to save it now?", "Save?",
                                             MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Yes)
                    Save();
                else if (result == DialogResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
            }
            ContentBuilder.Clear();
            ContentBuilder.Dispose();
        }

        private bool CheckContinue()
        {
            if (Saved)
                return true;
            var result = MessageBox.Show("File is not saved, do You want to save it now?", "Save sprite?",
                                         MessageBoxButtons.YesNoCancel);
            if (result == DialogResult.Yes)
                Save();
            else if (result == DialogResult.Cancel)
                return false;
            return true;
        }

        public static void DeleteDirectory(string path)
        {
            string[] files = Directory.GetFiles(path);
            string[] dirs = Directory.GetDirectories(path);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            try
            {
                Directory.Delete(path, false);
            }

            catch (IOException)
            {
                Thread.Sleep(0);
                Directory.Delete(path, false);
            }
        }


        private void ImportSprite(string fname)
        {
            var stream = new GZipStream(File.Open(fname, FileMode.Open), CompressionMode.Decompress);
            var buffer = new byte[sizeof (int)];
            stream.Read(buffer, 0, buffer.Length);
            var length = BitConverter.ToInt32(buffer, 0);
            buffer = new byte[length];
            stream.Read(buffer, 0, buffer.Length);
            var spriteName = Encoding.UTF8.GetString(buffer);
            var dirs = Directory.GetDirectories(RootPath);
            var found = false;
            foreach (var dir in dirs)
            {
                var v = Path.GetFileName(dir);
                if (v.ToLower() == spriteName.ToLower())
                {
                    found = true;
                    break;
                }
            }
            if (found)
            {
                if (
                    MessageBox.Show("Sprite '" + spriteName + "' already exist. Overwrite?", "Warning",
                                    MessageBoxButtons.YesNo) != DialogResult.Yes)
                {
                    stream.Close();
                    return;
                }
                ActiveSpriteWindow.CloseSprite();
                DeleteSprite(spriteName);
            }
            else
                ActiveSpriteWindow.CloseSprite();
            SpriteName = spriteName;
            _filename = SpriteName + ".xml";
            SpritePath = Path.Combine(RootPath, SpriteName);
            AnimationsPath = Path.Combine(SpritePath, "Animations");
            toolStripDeleteSpriteButton.Enabled = true;
            Saved = true;
            buffer = new byte[sizeof(int)];
            stream.Read(buffer, 0, buffer.Length);
            length = BitConverter.ToInt32(buffer, 0);
            buffer = new byte[length];
            stream.Read(buffer, 0, buffer.Length);
            Directory.CreateDirectory(SpritePath);
            Directory.CreateDirectory(Path.Combine(ContentPath, SpriteName));
            File.WriteAllBytes(Path.Combine(SpritePath, _filename), buffer);
            buffer = new byte[sizeof(int)];
            stream.Read(buffer, 0, buffer.Length);
            var animationsCount = BitConverter.ToInt32(buffer, 0);
            Directory.CreateDirectory(AnimationsPath);
            Directory.CreateDirectory(Path.Combine(ContentPath, SpriteName, "Animations"));
            for (int i = 0; i < animationsCount; i++)
            {
                buffer = new byte[sizeof(int)];
                stream.Read(buffer, 0, buffer.Length);
                length = BitConverter.ToInt32(buffer, 0);
                buffer = new byte[length];
                stream.Read(buffer, 0, buffer.Length);
                var imgName = Encoding.UTF8.GetString(buffer);
                buffer = new byte[sizeof(int)];
                stream.Read(buffer, 0, buffer.Length);
                length = BitConverter.ToInt32(buffer, 0);
                buffer = new byte[length];
                stream.Read(buffer, 0, buffer.Length);
                File.WriteAllBytes(Path.Combine(AnimationsPath, imgName), buffer);
                var xnbName = Path.Combine(ContentPath, SpriteName, "Animations", Path.GetFileNameWithoutExtension(imgName) + ".xnb");
                buffer = new byte[sizeof(int)];
                stream.Read(buffer, 0, buffer.Length);
                length = BitConverter.ToInt32(buffer, 0);
                buffer = new byte[length];
                stream.Read(buffer, 0, buffer.Length);
                File.WriteAllBytes(xnbName, buffer);
            }
            stream.Close();
            OpenSprite(Path.Combine(SpritePath, _filename));
        }

        private void ToolStripOpenButtonClick(object sender, EventArgs e)
        {
            if (!CheckContinue())
                return;
            if (openFileDialog1.ShowDialog() != DialogResult.OK)
                return;
            if (Path.GetExtension(openFileDialog1.FileName) == ".spr")
            {
                ImportSprite(openFileDialog1.FileName);
                return;
            }
            if (!openFileDialog1.FileName.Contains(RootPath))
            {
                MessageBox.Show("Sprite file must be within project path(" + RootPath + "')");
                return;
            }
            SpriteName = Path.GetFileNameWithoutExtension(openFileDialog1.FileName);
            _filename = SpriteName + ".xml";
            SpritePath = Path.Combine(RootPath, SpriteName);
            AnimationsPath = Path.Combine(SpritePath, "Animations");
            OpenSprite(openFileDialog1.FileName);
        }

        private void OpenSprite(string fname)
        {
            var editorContent = EngineGlobals.ContentCache;
            EngineGlobals.ContentCache = Content;
            ActiveSpriteWindow.LoadSprite(fname);
            EngineGlobals.ContentCache = editorContent;
            toolStripImportAnimationButton.Enabled = true;
            toolStripExportButton.Enabled = true;
            toolStripDeleteSpriteButton.Enabled = true;
            Saved = true;
        }

        private void SpriteEditorShown(object sender, EventArgs e)
        {
            ActiveSpriteWindow.spriteWindow.BackColor = SpriteEditorSettings.Default.BackgroundColor;

        }

        private void ToolStrip1ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void ToolStripSaveButtonClick(object sender, EventArgs e)
        {
            Save();
        }

        private void DeleteSprite(string name)
        {
            var contentSpritePath = Path.Combine(ContentPath, name);
            var animationPath = Path.Combine(RootPath, name, "Animations");
            DeleteDirectory(animationPath);
            var spritePath = Path.Combine(RootPath, name);
            File.Delete(Path.Combine(spritePath, name + ".xml"));
            DeleteDirectory(spritePath);
            DeleteDirectory(Path.Combine(contentSpritePath, "Animations"));
            DeleteDirectory(contentSpritePath);
        }

        private void ToolStripDeleteSpriteButtonClick(object sender, EventArgs e)
        {
            if (
                MessageBox.Show("Permanently remove sprite '" + SpriteName + "'?", "Remove Sprite",
                                MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                toolStripSaveButton.Enabled = false;
                toolStripDeleteSpriteButton.Enabled = false;
                toolStripImportAnimationButton.Enabled = false;
                Saved = true;
                ActiveSpriteWindow.CloseSprite();
                DeleteSprite(SpriteName);
            }
        }

        private void ToolStripImportSpriteButtonClick(object sender, EventArgs e)
        {

        }

        private void ToolStripExportButtonClick(object sender, EventArgs e)
        {
            saveFileDialog1.FileName = SpriteName + ".spr";
            if (saveFileDialog1.ShowDialog() != DialogResult.OK)
                return;
            if (!Saved)
            {
                var result = MessageBox.Show("Sprite data not saved do You want to save it now?", "Save?",
                                             MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Yes)
                    Save();
                else if (result == DialogResult.Cancel)
                {
                    return;
                }
            }
            var stream = new GZipStream(File.Open(saveFileDialog1.FileName, FileMode.Create), CompressionMode.Compress);
            var buffer = BitConverter.GetBytes(SpriteName.Length);
            stream.Write(buffer, 0, buffer.Length);
            buffer = Encoding.UTF8.GetBytes(SpriteName);
            stream.Write(buffer, 0, buffer.Length);
            var fileBuffer = File.ReadAllBytes(Path.Combine(SpritePath, SpriteName + ".xml"));
            buffer = BitConverter.GetBytes(fileBuffer.Length);
            stream.Write(buffer, 0, buffer.Length);
            stream.Write(fileBuffer, 0, fileBuffer.Length);
            buffer = BitConverter.GetBytes(ActiveSpriteWindow.Root.Animations);
            stream.Write(buffer, 0, buffer.Length);
            foreach (var animation in ActiveSpriteWindow.Root.Sprite.Data.Animations)
            {
                var imgFile = Directory.GetFiles(AnimationsPath,
                               animation.Name + ".*").First();
                var name = Path.GetFileName(imgFile);
                buffer = BitConverter.GetBytes(name.Length);
                stream.Write(buffer, 0, buffer.Length);
                buffer = Encoding.UTF8.GetBytes(name);
                stream.Write(buffer, 0, buffer.Length);
                fileBuffer = File.ReadAllBytes(imgFile);
                buffer = BitConverter.GetBytes(fileBuffer.Length);
                stream.Write(buffer, 0, buffer.Length);
                stream.Write(fileBuffer, 0, fileBuffer.Length);
                var xnbName = Path.Combine(ContentPath, SpriteName, "Animations", animation.Name + ".xnb");
                fileBuffer = File.ReadAllBytes(xnbName);
                buffer = BitConverter.GetBytes(fileBuffer.Length);
                stream.Write(buffer, 0, buffer.Length);
                stream.Write(fileBuffer, 0, fileBuffer.Length);
            }
            stream.Close();

        }
    }
}
