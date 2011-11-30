using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TileEditor
{
    public class TileDisplay : GraphicsDeviceControl
    {
        public event EventHandler DeviceDraw;
        public event EventHandler DeviceInitialize;

        protected virtual void OnDraw()
        {
            if (DeviceDraw != null)
            {
                DeviceDraw(this, EventArgs.Empty);
            }
        }

        protected virtual void OnDeviceInitialize()
        {
            if (DeviceInitialize != null)
            {
                DeviceInitialize(this, EventArgs.Empty);
            }
        }

        protected override void Initialize()
        {
            OnDeviceInitialize();
        }

        protected override void Draw()
        {
            OnDraw();
        }
    }
}
