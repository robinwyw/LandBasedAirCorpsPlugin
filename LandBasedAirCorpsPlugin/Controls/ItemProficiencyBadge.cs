using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LandBasedAirCorpsPlugin.Controls
{
    public class ItemProficiencyBadge : Control
    {
        static ItemProficiencyBadge()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ItemProficiencyBadge), new FrameworkPropertyMetadata(typeof(ItemProficiencyBadge)));
        }

        #region Proficiency dependency property

        public int Proficiency
        {
            get { return (int)this.GetValue(ProficiencyProperty); }
            set { this.SetValue(ProficiencyProperty, value); }
        }

        public static readonly DependencyProperty ProficiencyProperty =
            DependencyProperty.Register("Proficiency", typeof(int), typeof(ItemProficiencyBadge), new PropertyMetadata(0));

        #endregion

        #region IsBackgroundTransparent dependency property

        public bool IsBackgroundTransparent
        {
            get { return (bool)GetValue(IsBackgroundTransparentProperty); }
            set { SetValue(IsBackgroundTransparentProperty, value); }
        }

        public static readonly DependencyProperty IsBackgroundTransparentProperty =
            DependencyProperty.Register("IsBackgroundTransparent", typeof(bool), typeof(ItemProficiencyBadge), new PropertyMetadata(false));

        #endregion

    }
}
