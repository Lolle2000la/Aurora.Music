﻿// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Aurora.Music.Core;
using Aurora.Music.ViewModels;
using Aurora.Shared.Extensions;
using ExpressionBuilder;
using System.Numerics;
using Windows.System.Threading;
using System;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using EF = ExpressionBuilder.ExpressionFunctions;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using System.Linq;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Aurora.Music.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ArtistsPage : Page
    {
        private ArtistViewModel _clickedArtist;

        public ArtistsPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;

            var t = ThreadPool.RunAsync(async x =>
            {
                await Context.GetArtists();
            });
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!Context.ArtistList.IsNullorEmpty() && _clickedArtist != null)
            {
                ArtistList.ScrollIntoView(_clickedArtist);
                var ani = ConnectedAnimationService.GetForCurrentView().GetAnimation(Consts.ArtistPageInAnimation + "_1");
                if (ani != null)
                {
                    await ArtistList.TryStartConnectedAnimationAsync(ani, _clickedArtist, "ArtistName");
                }
                ani = ConnectedAnimationService.GetForCurrentView().GetAnimation(Consts.ArtistPageInAnimation + "_2");
                if (ani != null)
                {
                    await ArtistList.TryStartConnectedAnimationAsync(ani, _clickedArtist, "ArtistImage");
                }
                return;
            }
        }

        private void ArtistCell_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            //if (sender is FrameworkElement s)
            //{
            //    (s.Resources["PointerOver"] as Storyboard).Begin();
            //}
        }

        private void ArtistCell_PointerExited(object sender, PointerRoutedEventArgs e)
        {
        }

        private void ArtistCell_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
        }

        private void ArtistCell_PointerReleased(object sender, PointerRoutedEventArgs e)
        {

        }

        private void AlbumList_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void ArtistList_ItemClick(object sender, ItemClickEventArgs e)
        {
            ArtistList.PrepareConnectedAnimation(Consts.ArtistPageInAnimation + "_1", e.ClickedItem, "ArtistName");
            ArtistList.PrepareConnectedAnimation(Consts.ArtistPageInAnimation + "_2", e.ClickedItem, "ArtistImage");

            LibraryPage.Current.Navigate(typeof(ArtistPage), (e.ClickedItem as ArtistViewModel));
            _clickedArtist = e.ClickedItem as ArtistViewModel;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var p = (string)((sender as ComboBox).SelectedItem as ComboBoxItem).Tag;
            Context.ChangeSort(p);
        }

        private void ArtistList_ContextRequested(UIElement sender, ContextRequestedEventArgs args)
        {
            // Walk up the tree to find the ListViewItem.
            // There may not be one if the click wasn't on an item.
            var requestedElement = (FrameworkElement)args.OriginalSource;
            while ((requestedElement != sender) && !(requestedElement is SelectorItem))
            {
                requestedElement = (FrameworkElement)VisualTreeHelper.GetParent(requestedElement);
            }
            var model = (sender as ListViewBase).ItemFromContainer(requestedElement) as ArtistViewModel;
            if (requestedElement != sender)
            {
                var albumMenu = MainPage.Current.SongFlyout.Items.First(x => x.Name == "AlbumMenu") as MenuFlyoutItem;
                albumMenu.Visibility = Visibility.Collapsed;

                // remove performers in flyout
                var index = MainPage.Current.SongFlyout.Items.IndexOf(albumMenu);
                while (!(MainPage.Current.SongFlyout.Items[index + 1] is MenuFlyoutSeparator))
                {
                    MainPage.Current.SongFlyout.Items.RemoveAt(index + 1);
                }

                if (args.TryGetPosition(requestedElement, out var point))
                {
                    MainPage.Current.SongFlyout.ShowAt(requestedElement, point);
                }
                else
                {
                    MainPage.Current.SongFlyout.ShowAt(requestedElement);
                }

                args.Handled = true;
            }
        }

        private void ArtistList_ContextCanceled(UIElement sender, RoutedEventArgs args)
        {
            MainPage.Current.SongFlyout.Hide();
        }
    }
}
