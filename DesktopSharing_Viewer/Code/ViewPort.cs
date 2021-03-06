﻿using Desktop_Sharing_Shared.Screen;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DesktopSharing_Viewer.Code
{
    public class ViewPort : Panel
    {
        private Bitmap _Image = null;
        public Bitmap Image
        {
            get
            {
                return _Image;
            }
            set
            {
                if(_Image != null)
                    _Image.Dispose();
                _Image = value;
                if(_Image != null)
                {
                    Size = _Image.Size;
                }

            }
        }
        private List<Raw_Image> _BitsReceived = new List<Raw_Image>();

        public Image _Mouse_Image = null;
        public Image Mouse_Image
        {
            get
            {
                return _Mouse_Image;
            }
            set
            {
                if(_Mouse_Image != null)
                    _Mouse_Image.Dispose();
                _Mouse_Image = value;
            }
        }
        public Point Mouse_Position = new Point(0, 0);
        private object _BitsReceivedLock = new object();
        public void UpdateRegion(Rectangle p, byte[] m)
        {
            lock(_BitsReceivedLock)
            {
                _BitsReceived.Add(new Raw_Image { Data = m, Dimensions = p });
            }

        }

        //protected override CreateParams CreateParams
        //{
        //    get
        //    {
        //        CreateParams cp = base.CreateParams;
        //        cp.ExStyle |= 0x00000020; //WS_EX_TRANSPARENT 

        //        return cp;
        //    }
        //}
        public void New_Image(Point sz, byte[] m)
        {
            using(var ms = new MemoryStream(m))
            {
                Image = new Bitmap(ms);
            }
            

             //Desktop_Sharing_Shared.Bitmap_Helper.Fill(timg, sz, m);

             //unsafe
             //{
             //    fixed(byte* datb = m)
             //    {
             //        using(Bitmap image = new Bitmap(sz.X, sz.Y, sz.X * 4,
             //                 System.Drawing.Imaging.PixelFormat.Format32bppRgb, new IntPtr(datb)))
             //        {

             //            image.Save(@"C:\Users\scott\Desktop\text.png", System.Drawing.Imaging.ImageFormat.Png);
             //        }
             //    }
             //}
           

        }
        public ViewPort()
        {

            this.SetStyle(ControlStyles.Selectable, true);
            MouseEnter += ViewPort_MouseEnter;
            MouseLeave += ViewPort_MouseLeave;
            //SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            //SetStyle(ControlStyles.ContainerControl, false);
            //SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            //SetStyle(ControlStyles.Opaque, true);

        }
        void ViewPort_MouseLeave(object sender, EventArgs e)
        {
            Cursor.Show();
        }

        void ViewPort_MouseEnter(object sender, EventArgs e)
        {
            Cursor.Hide();
        }
        protected override void OnClick(EventArgs e)
        {
            this.Focus();
            base.OnClick(e);
        }
        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            // Don't paint background
        }
        static int counter = 0;
        DateTime date = DateTime.Now;
        protected override void OnPaint(PaintEventArgs e)
        {
            var dt = new Stopwatch();
            dt.Start();
            //if(_Image != null)
            //{
            //    e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low;
            //    e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Default;
            //    e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;

            //    foreach(var item in _BitsReceivedOld)
            //        item.Image.Dispose();
            //    _BitsReceivedOld = System.Threading.Interlocked.Exchange(ref _BitsReceived, new List<BitMapHelper>());

            //    e.Graphics.DrawImageUnscaled(_Image, new Point(0, 0));
            //    foreach(var item in _BitsReceivedOld)
            //    {
            //        e.Graphics.DrawImageUnscaled(item.Image, item.TL);

            //        Desktop_Sharing_Shared.Bitmap_Helper.Copy(item.Image, _Image, item.TL);
            //        item.Image.Dispose();
            //    }
            //    _BitsReceived.Clear();

            //    if(Mouse_Image != null)
            //        e.Graphics.DrawImageUnscaled(Mouse_Image, Mouse_Position);


            //    dt.Stop();
            //    Debug.WriteLine("OnPaint (1): " + dt.ElapsedMilliseconds);
            //}
            int updates = 0;
            try
            {

                if((DateTime.Now - date).TotalMilliseconds > 1000)
                {
                    date = DateTime.Now;
                    Debug.WriteLine("fps: " + counter);
                    counter = 0;
                }
                counter++;
                
                if(_Image != null)
                {
                    var tmpimgs = new List<Raw_Image>();
                    lock(_BitsReceivedLock)
                    {
                        var t = _BitsReceived;
                        _BitsReceived = tmpimgs;
                        tmpimgs = t;
                    }
                    e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low;
                    e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Default;
                    e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                    updates = tmpimgs.Count();

                    Desktop_Sharing_Shared.Bitmap_Helper.Copy(tmpimgs, _Image);

                    e.Graphics.DrawImageUnscaled(_Image, new Point(0, 0));
                    if(Mouse_Image != null)
                        e.Graphics.DrawImageUnscaled(Mouse_Image, Mouse_Position);


                    //dt.Stop();
                    //Debug.WriteLine("GDI write (1): " + dt.ElapsedMilliseconds);
                    //dt.Start();
                }
            } catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            dt.Stop();
            Debug.WriteLine("OnPaint (1): " + dt.ElapsedMilliseconds + "  frames: " + updates);

        }
    }
}
