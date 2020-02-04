// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutoWorkspace.cs" company="WildGums">
//   Copyright (c) 2008 - 2014 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement.Behaviors
{
    using System.Windows;

    public class AutoWorkspace : WorkspaceBehaviorBase<FrameworkElement>
    {
        #region Properties
        public bool PersistSize
        {
            get { return (bool)GetValue(PersistSizeProperty); }
            set { SetValue(PersistSizeProperty, value); }
        }

        public static readonly DependencyProperty PersistSizeProperty =
            DependencyProperty.Register("PersistSize", typeof(bool), typeof(AutoWorkspace), new PropertyMetadata(true));


        public bool PersistGridSettings
        {
            get { return (bool)GetValue(PersistGridSettingsProperty); }
            set { SetValue(PersistGridSettingsProperty, value); }
        }

        public static readonly DependencyProperty PersistGridSettingsProperty =
            DependencyProperty.Register("PersistGridSettings", typeof(bool), typeof(AutoWorkspace), new PropertyMetadata(true));
        #endregion

        #region Methods

        protected override void SaveSettings(IWorkspace workspace, string prefix)
        {
            if (PersistSize)
            {
                AssociatedObject.SaveSizeToWorkspace(workspace, prefix);
            }

            if (PersistGridSettings)
            {
                AssociatedObject.SaveGridValuesToWorkspace(workspace, prefix);
            }
        }

        protected override void LoadSettings(IWorkspace workspace, string prefix)
        {
            if (PersistSize)
            {
                AssociatedObject.LoadSizeFromWorkspace(workspace, prefix);
            }

            if (PersistGridSettings)
            {
                AssociatedObject.LoadGridValuesFromWorkspace(workspace, prefix);
            }
        }
        #endregion
    }
}
