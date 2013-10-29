using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace DBAccess
{
    //
    //
    //
    public class MapPan
    {
        public void Start(List<Tool.Point> refPos)
        {
            refPositions = refPos;
            offset = Tool.Size.Empty;
            lastPositionMouse = Form.MousePosition;
        }
        public void Start(Point refPos)
        {
            refPositions = new List<Tool.Point>();
            refPositions.Add(refPos);
            offset = Tool.Size.Empty;
            lastPositionMouse = Form.MousePosition;
        }
        public void Update()
        {
            offset = Form.MousePosition - lastPositionMouse;
        }
        public void Stop()
        {
            offset = Tool.Size.Empty;
        }

        public Tool.Size Offset { get { return offset; } }
        public Tool.Point Position(int idx) { return refPositions[idx] + offset; }

        private List<Tool.Point> refPositions;
        private Tool.Size offset = Tool.Size.Empty;
        private Tool.Point lastPositionMouse;
    }
}
