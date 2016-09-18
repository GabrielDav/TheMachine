using System;
using System.Windows.Forms;

namespace GameEditor.Sprite_Editor
{
// ReSharper disable ClassNeverInstantiated.Global
    public class TabControlMod : TabControl
// ReSharper restore ClassNeverInstantiated.Global
    {
        public bool HideTabPageButtons { get; set; }

        protected override void WndProc(ref Message m)
        {
            // Hide tabs by trapping the TCM_ADJUSTRECT message
            if (m.Msg == 0x1328 && !DesignMode && HideTabPageButtons) m.Result = (IntPtr)1;
            else base.WndProc(ref m);
        }

        public TabPage AddTab()
        {
            TabPages.Add("tabPage"+TabPages.Count);
            var spriteControl = new SpriteControl
                {
                    Dock = DockStyle.Fill,
                    Location = new System.Drawing.Point(3, 3),
                    Name = "spriteControl1",
                    Size = new System.Drawing.Size(798, 488),
                    TabIndex = TabPages.Count - 1
                };
            TabPages[TabPages.Count - 1].Controls.Add(spriteControl);
            return TabPages[TabPages.Count - 1];
        }
    }

}
