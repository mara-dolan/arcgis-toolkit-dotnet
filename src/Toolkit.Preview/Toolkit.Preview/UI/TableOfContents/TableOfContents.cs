﻿// /*******************************************************************************
//  * Copyright 2012-2018 Esri
//  *
//  *  Licensed under the Apache License, Version 2.0 (the "License");
//  *  you may not use this file except in compliance with the License.
//  *  You may obtain a copy of the License at
//  *
//  *  http://www.apache.org/licenses/LICENSE-2.0
//  *
//  *   Unless required by applicable law or agreed to in writing, software
//  *   distributed under the License is distributed on an "AS IS" BASIS,
//  *   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  *   See the License for the specific language governing permissions and
//  *   limitations under the License.
//  ******************************************************************************/

#if !__IOS__ && !__ANDROID__ && !NETSTANDARD2_0 && !NETFX_CORE

using System.Windows;
#if NETFX_CORE
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#else
using System.Windows.Controls;
#endif
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.UI.Controls;

namespace Esri.ArcGISRuntime.Toolkit.Preview.UI.Controls
{
    /// <summary>
    /// A control that creates a table of content tree view from a <see cref="GeoView"/>.
    /// </summary>
    [TemplatePart(Name = "TreeView", Type = typeof(ItemsControl))]
    public class TableOfContents : Control
    {
        private readonly TocDataSource _datasource;

        /// <summary>
        /// Initializes a new instance of the <see cref="TableOfContents"/> class.
        /// </summary>
        public TableOfContents()
            : base()
        {
            DefaultStyleKey = typeof(TableOfContents);
            _datasource = new TocDataSource(this);
        }

        /// <inheritdoc />
#if NETFX_CORE
        protected override void OnApplyTemplate()
#else
        public override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();
            ItemTemplateSelector = new TocItemTemplateSelector(this);
            UpdateDatasource();
        }

        private void UpdateDatasource()
        {
            var tree = GetTemplateChild("TreeView") as ItemsControl;
            if (tree != null)
            {
                tree.ItemsSource = _datasource;
#if !NETFX_CORE
                ContextMenuService.AddContextMenuOpeningHandler(tree, ContextMenuEventHandler);
#endif
            }
        }

        /// <summary>
        /// Gets or sets the geoview that contain the layers whose symbology and description will be displayed.
        /// </summary>
        /// <seealso cref="MapView"/>
        /// <seealso cref="SceneView"/>
        public GeoView GeoView
        {
            get { return (GeoView)GetValue(GeoViewProperty); }
            set { SetValue(GeoViewProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="GeoView"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GeoViewProperty =
            DependencyProperty.Register(nameof(GeoView), typeof(GeoView), typeof(TableOfContents), new PropertyMetadata(null, OnGeoViewPropertyChanged));

        private static void OnGeoViewPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var toc = (TableOfContents)d;
            toc._datasource.SetGeoView(e.NewValue as GeoView);
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show a legend for the layers in the tree view
        /// </summary>
        public bool ShowLegend
        {
            get { return (bool)GetValue(ShowLegendProperty); }
            set { SetValue(ShowLegendProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="ShowLegend"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowLegendProperty =
            DependencyProperty.Register(nameof(ShowLegend), typeof(bool), typeof(TableOfContents), new PropertyMetadata(true, OnShowLegendPropertyChanged));

        private static void OnShowLegendPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var toc = (TableOfContents)d;
            toc._datasource.ShowLegend = (bool)e.NewValue;
        }

        /// <summary>
        /// Gets or sets the item template for each <see cref="ILayerContent" /> entry.
        /// </summary>
        /// <remarks>
        /// If this is set to null, the <see cref="SublayerItemTemplate"/> will be used instead.
        /// </remarks>
        /// <seealso cref="SublayerItemTemplate"/>
        /// <seealso cref="LegendInfoItemTemplate"/>
        /// <seealso cref="BasemapItemTemplate"/>
        public DataTemplate LayerItemTemplate
        {
            get { return (DataTemplate)GetValue(LayerItemTemplateProperty); }
            set { SetValue(LayerItemTemplateProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="LayerItemTemplate"/> dependency property.
        /// </summary>
        /// <remarks>
        /// If this is set to null, the <see cref="SublayerItemTemplate"/> will be used instead.
        /// </remarks>
        public static readonly DependencyProperty LayerItemTemplateProperty =
            DependencyProperty.Register(nameof(LayerItemTemplate), typeof(DataTemplate), typeof(TableOfContents), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the item template for each <see cref="ILayerContent"/> entry that is not a <see cref="Layer"/>.
        /// </summary>
        /// <seealso cref="LegendInfoItemTemplate"/>
        /// <seealso cref="LayerItemTemplate"/>
        /// <seealso cref="BasemapItemTemplate"/>
        public DataTemplate SublayerItemTemplate
        {
            get { return (DataTemplate)GetValue(SublayerItemTemplateProperty); }
            set { SetValue(SublayerItemTemplateProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="SublayerItemTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SublayerItemTemplateProperty =
                DependencyProperty.Register(nameof(SublayerItemTemplate), typeof(DataTemplate), typeof(TableOfContents), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the item template for each <see cref="LegendInfo"/> content entry.
        /// </summary>
        /// <seealso cref="SublayerItemTemplate"/>
        /// <seealso cref="LayerItemTemplate"/>
        /// <seealso cref="BasemapItemTemplate"/>
        public DataTemplate LegendInfoItemTemplate
        {
            get { return (DataTemplate)GetValue(LegendInfoItemTemplateProperty); }
            set { SetValue(LegendInfoItemTemplateProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="LegendInfoItemTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendInfoItemTemplateProperty =
                DependencyProperty.Register(nameof(LegendInfoItemTemplate), typeof(DataTemplate), typeof(TableOfContents), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the item template for each <see cref="Basemap"/> content entry.
        /// </summary>
        /// <seealso cref="SublayerItemTemplate"/>
        /// <seealso cref="LayerItemTemplate"/>
        /// <seealso cref="LegendInfoItemTemplate"/>
        public DataTemplate BasemapItemTemplate
        {
            get { return (DataTemplate)GetValue(BasemapItemTemplateProperty); }
            set { SetValue(BasemapItemTemplateProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="BasemapItemTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BasemapItemTemplateProperty =
                DependencyProperty.Register(nameof(BasemapItemTemplate), typeof(DataTemplate), typeof(TableOfContents), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the template selector used for selecting templates for each entry in the list.
        /// </summary>
        /// <remarks>
        /// If the default template selector is overridden, the item templates will not apply.
        /// </remarks>
        public DataTemplateSelector ItemTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(ItemTemplateSelectorProperty); }
            set { SetValue(ItemTemplateSelectorProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="ItemTemplateSelector"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemTemplateSelectorProperty =
                DependencyProperty.Register(nameof(ItemTemplateSelector), typeof(DataTemplateSelector), typeof(TableOfContents), new PropertyMetadata(null));

#if !NETFX_CORE
        private void ContextMenuEventHandler(object sender, ContextMenuEventArgs e)
        {
            (sender as FrameworkElement).ContextMenu = null;
            var vm = (e.OriginalSource as FrameworkElement)?.DataContext;
            TocItem item = null;
            LegendInfo info = null;
            if (vm is LegendInfo li)
            {
                info = li;
                var parent = System.Windows.Media.VisualTreeHelper.GetParent(e.OriginalSource as DependencyObject) as FrameworkElement;
                while (parent != null)
                {
                    if (parent.DataContext is TocItem tocItem)
                    {
                        item = tocItem;
                        break;
                    }

                    parent = System.Windows.Media.VisualTreeHelper.GetParent(parent) as FrameworkElement;
                }
            }

            if (vm is TocItem ti)
            {
                item = ti;
            }

            if (vm != null)
            {
                if (TableOfContentContextMenuOpening != null)
                {
                    var ctm = new ContextMenu();
                    var args = new TableOfContentsContextMenuEventArgs(sender, e)
                    {
                        MenuItems = ctm.Items,
                        TableOfContentItem = item?.Content,
                        LegendInfo = info,
                        Menu = ctm
                    };
                    TableOfContentContextMenuOpening?.Invoke(this, args);
                    e.Handled = args.Handled;
                    if (args.MenuItems.Count > 0)
                    {
                        (sender as FrameworkElement).ContextMenu = args.Menu;
                    }
                }
            }
        }

        /// <summary>
        /// Event fired by the <see cref="TableOfContents"/> when right-clicking an item
        /// </summary>
        public event System.EventHandler<TableOfContentsContextMenuEventArgs> TableOfContentContextMenuOpening;
#endif
    }
}
#endif