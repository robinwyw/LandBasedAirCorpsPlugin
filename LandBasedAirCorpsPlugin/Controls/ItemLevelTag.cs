using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LandBasedAirCorpsPlugin.Controls
{
    public class ItemLevelTag : Control
    {
        static ItemLevelTag()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ItemLevelTag), new FrameworkPropertyMetadata(typeof(ItemLevelTag)));
        }

        public string LevelText
        {
            get { return (string)GetValue(LevelTextProperty); }
            set { SetValue(LevelTextProperty, value); }
        }

        public static readonly DependencyProperty LevelTextProperty =
            DependencyProperty.Register("LevelText", typeof(string), typeof(ItemLevelTag), new PropertyMetadata(""));
    }
}
