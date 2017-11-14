﻿using Aurora.Shared.Extensions;
using Aurora.Shared.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml.Media.Imaging;

namespace Aurora.Music.ViewModels
{
    class AlbumDetailViewModel : ViewModelBase
    {
        private ObservableCollection<SongViewModel> songList;
        public ObservableCollection<SongViewModel> SongList
        {
            get { return songList; }
            set { SetProperty(ref songList, value); }
        }

        private AlbumViewModel album;
        public AlbumViewModel Album
        {
            get { return album; }
            set { SetProperty(ref album, value); }
        }

        private Uri heroImage;
        public Uri HeroImage
        {
            get { return heroImage; }
            set { SetProperty(ref heroImage, value); }
        }

        public string SongsCount(AlbumViewModel a)
        {
            if (a != null)
            {
                return a.SongsID.Length == 1 ? "1 Song" : $"{a.TrackCount} Songs";
            }
            return "0 Songs";
        }

        public string GenresToString(AlbumViewModel a)
        {
            if (a != null && !a.Genres.IsNullorEmpty())
            {
                return string.Join(", ", a.Genres);
            }
            return "Various Genres";
        }

        public DelegateCommand PlayAll
        {
            get
            {
                return new DelegateCommand(async () =>
                {
                    await MainPageViewModel.Current.InstantPlay(await Album.GetSongsAsync());
                });
            }
        }

        public AlbumDetailViewModel()
        {
            SongList = new ObservableCollection<SongViewModel>();
        }

        public async Task GetSongsAsync(AlbumViewModel a)
        {
            Album = a;
            await a.GetSongsAsync();
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
            {
                SongList.Clear();
                for (int i = 0; i < a.Songs.Count; i++)
                {
                    SongList.Add(new SongViewModel(a.Songs[i])
                    {
                        Index = (uint)i
                    });
                }
            });
        }

        internal async Task PlayAt(SongViewModel songViewModel)
        {
            await MainPageViewModel.Current.InstantPlay(await Album.GetSongsAsync(), songList.IndexOf(songViewModel));
        }
    }
}
