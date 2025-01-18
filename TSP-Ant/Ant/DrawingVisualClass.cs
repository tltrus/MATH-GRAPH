using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Controls;

namespace WpfApp
{
    class DrawingVisualClass : Canvas
    {
        List<Visual> visuals = new List<Visual>();

        protected override int VisualChildrenCount
        {
            get
            {
                return visuals.Count;
            }
        }

        protected override Visual GetVisualChild(int index)
        {
            return visuals[index];
        }

        public void AddVisual(Visual visual)
        {
            visuals.Add(visual);
            AddVisualChild(visual);
            AddLogicalChild(visual);
        }

        public void RemoveVisual(Visual visual)
        {
            visuals.Remove(visual);
            RemoveVisualChild(visual);
            RemoveLogicalChild(visual);
        }
    }
}
