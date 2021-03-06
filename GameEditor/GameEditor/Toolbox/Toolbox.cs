﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using GameEditor.Toolbox;

namespace GameEditor.ToolBox
{
    public class Toolbox : UserControl
    {

        private Dictionary<string, ToolboxGroup> _groups;
        private ImageList _images;
        private ToolboxItemBase _currentMouseOverItem;
        private ToolboxItem _selectedItem;
        private Color _groupColor;
        private Color _selectedItemColor;
        private Color _mouseOverColor;
        private Color _selectedMouseOverColor;
        private Color _disabledItemColor;
        private Color _itemBorderColor;
        private readonly ColorMatrix _matrix;

        public event EventHandler OnSelectedItemChanged;

        public Toolbox()
        {
            this.AutoScroll = true;
            _groups = new Dictionary<string, ToolboxGroup>();
            _images = new ImageList();
            _images.ColorDepth = ColorDepth.Depth32Bit;
            _images.ImageSize = new Size(16, 16);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            _groupColor = Color.FromArgb(240, 240, 240);
            _selectedItemColor = SystemColors.GradientInactiveCaption;
            _disabledItemColor = SystemColors.GrayText;
            _mouseOverColor = SystemColors.GradientActiveCaption;
            _selectedMouseOverColor = SystemColors.ActiveCaption;
            _itemBorderColor = SystemColors.HotTrack;
            this.BackColor = Color.FromArgb(225, 225, 225);
            this.BorderStyle = BorderStyle.FixedSingle;
            _matrix = new ColorMatrix();
            _matrix[0, 0] = 1 / 3f;
            _matrix[0, 1] = 1 / 3f;
            _matrix[0, 2] = 1 / 3f;
            _matrix[1, 0] = 1 / 3f;
            _matrix[1, 1] = 1 / 3f;
            _matrix[1, 2] = 1 / 3f;
            _matrix[2, 0] = 1 / 3f;
            _matrix[2, 1] = 1 / 3f;
            _matrix[2, 2] = 1 / 3f;
        }

        [Category("Colors")]
        public Color ItemBorderColor
        {
            get
            {
                return _itemBorderColor;
            }
            set
            {
                _itemBorderColor = value;
            }
        }

        [Category("Colors")]
        public Color SelectedMouseOverColor
        {
            get
            {
                return _selectedMouseOverColor;
            }
            set
            {
                _selectedMouseOverColor = value;
            }
        }

        [Category("Colors")]
        public Color MouseOverColor
        {
            get
            {
                return _mouseOverColor;
            }
            set
            {
                _mouseOverColor = value;
            }
        }

        [Category("Colors")]
        public Color GroupColor
        {
            get
            {
                return _groupColor;
            }
            set
            {
                _groupColor = value;
            }
        }

        [Category("Colors")]
        public Color SelectedItemColor
        {
            get
            {
                return _selectedItemColor;
            }
            set
            {
                _selectedItemColor = value;
            }
        }

        [Category("Colors")]
        public Color DisabledItemColor
        {
            get { return _disabledItemColor; }
            set { _disabledItemColor = value; }
        }


        protected override void OnScroll(ScrollEventArgs se)
        {
            base.OnScroll(se);
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {

            Debug.Print(e.ClipRectangle.ToString());

            SolidBrush backgroundBrush = new SolidBrush(this.BackColor);

            e.Graphics.FillRectangle(backgroundBrush, this.ClientRectangle);

            int offset = this.AutoScrollPosition.Y;
            foreach (ToolboxGroup group in _groups.Values)
            {
                PaintGroup(e.Graphics, group, backgroundBrush, ref offset);
            }

            backgroundBrush.Dispose();

            this.AutoScrollMinSize = new Size(this.Width - 30, offset - this.AutoScrollPosition.Y);

            base.OnPaint(e);

        }

        private void PaintGroup(Graphics graphics, ToolboxGroup group, SolidBrush backgroundBrush, ref int offset)
        {

            group.Top = offset;
            offset += 19;

            SolidBrush groupBrush = new SolidBrush(_groupColor);

            Rectangle groupRect = new Rectangle(1, group.Top, this.Width - 1, 18);
            graphics.FillRectangle(groupBrush, groupRect);

            groupBrush.Dispose();

            Pen backgroundPen = new Pen(backgroundBrush);

            int lineLocation = group.Top + 16;
            graphics.DrawLine(backgroundPen,
                              1,
                              lineLocation,
                              this.Width - 1,
                              lineLocation);

            backgroundPen.Dispose();

            graphics.DrawString(group.Caption,
                                this.Font,
                                Brushes.Black,
                                new RectangleF(20, group.Top + 2, this.Width - 30, group.Top + 13));

            if (group.Expanded)
            {
                graphics.DrawImage(Resources.Minus, new Point(6, group.Top + 4));
                foreach (ToolboxItem item in group.Items)
                {
                    PaintItem(graphics, item, backgroundBrush, ref offset);
                }
            }
            else
            {
                graphics.DrawImage(Resources.Plus, new Point(6, group.Top + 4));
                // ReSharper disable SuggestUseVarKeywordEverywhere
                foreach (ToolboxItem item in group.Items)
                    // ReSharper restore SuggestUseVarKeywordEverywhere
                {
                    item.Top = -1;
                }
            }

        }

        private void PaintItem(Graphics graphics, ToolboxItem item, SolidBrush backgroundBrush, ref int offset)
        {
            item.Top = offset;
            offset += 19;

            SolidBrush itemBrush = null;
            if (item.Enabled)
            {
                if (item.MouseOver && item.Selected)
                {
                    itemBrush = new SolidBrush(_selectedMouseOverColor);
                }
                else if (item.MouseOver)
                {
                    itemBrush = new SolidBrush(_mouseOverColor);
                }
                else if (item.Selected)
                {
                    itemBrush = new SolidBrush(_selectedItemColor);
                }
            }

            if (itemBrush != null)
            {
                PaintItemBackground(graphics, itemBrush, item.Top);
            }

            if (_images != null &&
                item.IconIndex >= 0 &&
                item.IconIndex < _images.Images.Count)
            {
                if (item.Enabled)
                {
                    _images.Draw(graphics,
                                 new Point(8, item.Top + 1),
                                 item.IconIndex);
                }
                else
                {
                    
                   
                    var attributes = new ImageAttributes();
                    attributes.SetColorMatrix(_matrix);
                    var img = _images.Images[item.IconIndex];
                    graphics.DrawImage(img, new Rectangle(8, item.Top + 1, 16, 16), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, attributes);
                }
            }

            if (item.Caption.Length > 0)
            {
                graphics.DrawString(item.Caption,
                    this.Font,
                    Enabled&&item.Enabled?Brushes.Black:new SolidBrush(_disabledItemColor),
                    new RectangleF(26, item.Top + 2, Width - 30, item.Top + 13));
            }

        }

        private void PaintItemBackground(Graphics graphics, Brush brush, int offset)
        {

            Rectangle itemRect = new Rectangle(0, offset, this.Width - 1, 18);

            graphics.FillRectangle(brush,
                itemRect);

            Pen pen = new Pen(_itemBorderColor);

            graphics.DrawRectangle(pen,
                itemRect);

            pen.Dispose();

        }

        [Browsable(false)]
        public Dictionary<string, ToolboxGroup> Groups
        {
            get
            {
                return _groups;
            }
        }

        [Category("Behavior")]
        public ImageList ImageList
        {
            get
            {
                return _images;
            }
            set
            {
                _images = value;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            ToolboxItemBase item = HitTest(e.Location);

            if (e.Button == MouseButtons.Left && item is ToolboxItem)
            {
                ToolboxItem toolboxItem = item as ToolboxItem;
                if (toolboxItem.TypeInfo != null && toolboxItem.Enabled)
                {
                    this.DoDragDrop(toolboxItem.TypeInfo, DragDropEffects.Copy);
                }
            }
            else
            {
                if (item != null && item.MouseOver == false)
                {
                    if (_currentMouseOverItem != null)
                    {
                        _currentMouseOverItem.MouseOver = false;
                        Invalidate(GetItemRect(_currentMouseOverItem));
                    }
                    item.MouseOver = true;
                    Invalidate(GetItemRect(item));
                    _currentMouseOverItem = item;
                }
            }

            base.OnMouseMove(e);
        }

        private Rectangle GetItemRect(ToolboxItemBase item)
        {
            return new Rectangle(0, item.Top, this.Width, 19);
        }

        private ToolboxItemBase HitTest(Point point)
        {
            foreach (ToolboxGroup group in _groups.Values)
            {
                if (PointOverToolboxItem(point, group))
                {
                    return group;
                }

                foreach (ToolboxItem item in group.Items)
                {
                    if (PointOverToolboxItem(point, item))
                    {
                        return item;
                    }
                }
            }

            return null;
        }

        private bool PointOverToolboxItem(Point point, ToolboxItemBase item)
        {
            if (item.Top == -1)
            {
                return false;
            }

            if (point.Y >= item.Top && point.Y <= item.Top + 18)
            {
                return true;
            }

            return false;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (_currentMouseOverItem != null)
            {
                _currentMouseOverItem.MouseOver = false;
                Invalidate(GetItemRect(_currentMouseOverItem));
                _currentMouseOverItem = null;
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            ToolboxItemBase item = HitTest(e.Location);

            if (item is ToolboxItem)
            {
                ItemMouseDown(item as ToolboxItem);
            }
            else if (item is ToolboxGroup)
            {
                GroupMouseDown(item as ToolboxGroup);
            }

            base.OnMouseDown(e);
        }

        private void GroupMouseDown(ToolboxGroup group)
        {
            if (group == null)
            {
                return;
            }

            group.Expanded = !group.Expanded;

            if (group.Expanded)
            {
                this.AutoScrollMinSize = new Size(this.Width - 30, this.AutoScrollMinSize.Height + group.ItemHeight);
            }
            else
            {
                this.AutoScrollMinSize = new Size(this.Width - 30, this.AutoScrollMinSize.Height - group.ItemHeight);
            }

            Invalidate(this.ClientRectangle);
        }

        private void ItemMouseDown(ToolboxItem item)
        {
            if (item.Selected == false)
            {
                if (_selectedItem != null)
                {
                    _selectedItem.Selected = false;
                    Invalidate(GetItemRect(_selectedItem));
                }
                item.Selected = true;
                Invalidate(GetItemRect(item));
                _selectedItem = item;

                if (OnSelectedItemChanged != null)
                {
                    OnSelectedItemChanged(this, EventArgs.Empty);
                }

            }
        }

        [Browsable(false)]
        public ToolboxItem SelectedItem
        {
            get
            {
                return _selectedItem;
            }
        }
    }
}
