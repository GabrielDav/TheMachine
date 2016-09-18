using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Engine.Graphics;
using XNAContentCompiler;

namespace GameEditor.Sprite_Editor
{
    public partial class ImportAnimationForm : Form
    {
        protected Sprite _sprite;
        protected SpriteEditor _owner;
        public int Frames;
        public int FrameWidth;
        public int FrameHeight;

        public ImportAnimationForm(Sprite sprite, SpriteEditor owner)
        {
            _sprite = sprite;
            _owner = owner;
            InitializeComponent();
            progressBar1.MarqueeAnimationSpeed = 20;
        }

        private void Button1Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
                textBox1.Text = openFileDialog1.FileName;
        }

        private void Button2Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text) || string.IsNullOrEmpty(textBox2.Text) || string.IsNullOrEmpty(textBox3.Text) || string.IsNullOrEmpty(textBox4.Text) || string.IsNullOrEmpty(textBox5.Text))
            {
                MessageBox.Show("Fill all empty fields.");
                return;
            }
            if (!int.TryParse(textBox3.Text, out FrameWidth))
            {
                MessageBox.Show("Incorrect Frame Width value.");
                return;
            }
            if (!int.TryParse(textBox4.Text, out FrameHeight))
            {
                MessageBox.Show("Incorrect Frame Height value.");
                return;
            }
            if (!int.TryParse(textBox5.Text, out Frames))
            {
                MessageBox.Show("Incorrect Frames value.");
                return;
            }
            if (!File.Exists(textBox1.Text))
            {
                MessageBox.Show("File '" + textBox1.Text + "' not found.");
                return;
            }
            
            foreach (var animation in _sprite.Data.Animations)
            {
                if (animation.Name.ToLower() == textBox2.Text.ToLower())
                {
                    MessageBox.Show("Animation with name '" + animation.Name + "' already exists.");
                    return;
                }
            }
            var worker = new BackgroundWorker();
            worker.DoWork += WorkerOnDoWork;
            Enabled = false;
            progressBar1.Show();
            worker.RunWorkerAsync();

        }

        private void WorkerOnDoWork(object sender, DoWorkEventArgs doWorkEventArgs)
        {
            string path = "";
            textBox1.Invoke(new MethodInvoker(delegate { path = textBox1.Text; }));
            string name = "";
            textBox2.Invoke(new MethodInvoker(delegate { name = textBox2.Text; }));
            File.Copy(path, Path.Combine(_owner.AnimationsPath, name + Path.GetExtension(path)));
            var tempOutputPath = Path.Combine(_owner.TempPath, _owner.SpriteName);
            _owner.ContentBuilder.Add(path, name);
            _owner.ContentBuilder.Build(tempOutputPath);
            _owner.ContentBuilder.Clear();
            File.Copy(Path.Combine(tempOutputPath, "content", textBox2.Text + ".xnb"), Path.Combine(_owner.ContentPath, _owner.SpriteName, "Animations", textBox2.Text + ".xnb"));
            Invoke(new MethodInvoker(() => { DialogResult = DialogResult.OK; Close(); }));
        }
    }
}
