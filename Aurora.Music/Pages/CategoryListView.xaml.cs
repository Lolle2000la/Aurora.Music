﻿using Aurora.Shared.MVVM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Aurora.Music.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class CategoryListView : Page
    {
        private static object clickedItem;

        public CategoryListView()
        {
            this.InitializeComponent();
        }

        CategoryListItem[] categoryList = { new CategoryListItem { Title = "Songs", Index = new Uri("ms-appx:///Assets/Images/1.png"), IsCurrent = true }, new CategoryListItem { Title = "Albums", Index = new Uri("ms-appx:///Assets/Images/2.png") }, new CategoryListItem { Title = "Artists", Index = new Uri("ms-appx:///Assets/Images/3.png") } };

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            PrepareAnimationWithItem(e.ClickedItem);
            clickedItem = e.ClickedItem;
            LibraryPage.Current.LefPanelNavigate(typeof(CategoryDetailsView), (e.ClickedItem as CategoryListItem).Title);
        }

        void PrepareAnimationWithItem(object item)
        {
            Category.PrepareConnectedAnimation("CategoryListIn", item, "Panel");
            Category.PrepareConnectedAnimation("CategoryTitleIn", item, "Title");
        }

        private void Category_Loaded(object sender, RoutedEventArgs e)
        {
            if (clickedItem != null)
            {
                Category.ScrollIntoView(clickedItem);
                var animation =
                    ConnectedAnimationService.GetForCurrentView().GetAnimation("CategoryListOut");
                var animation1 =
                   ConnectedAnimationService.GetForCurrentView().GetAnimation("CategoryTitleOut");
                if (animation != null)
                {
                    var a = Category.TryStartConnectedAnimationAsync(animation, clickedItem, "Panel");
                }
                if (animation1 != null)
                {
                    var b = Category.TryStartConnectedAnimationAsync(animation1, clickedItem, "Title");
                }
            }
        }
    }



    public class CategoryListItem : ViewModelBase
    {
        public string Title { get; set; }

        public Uri Index { get; set; }

        private bool isCurrent;
        public bool IsCurrent
        {
            get { return isCurrent; }
            set { SetProperty(ref isCurrent, value); }
        }

        public double GetHeight(bool b)
        {
            return b ? 192d : 96d;
        }
    }

}