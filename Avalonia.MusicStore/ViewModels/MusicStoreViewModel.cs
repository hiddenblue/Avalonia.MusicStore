using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.MusicStore.Models;
using System.Threading;
using System.Reactive;

namespace Avalonia.MusicStore.ViewModels
{
    public class MusicStoreViewModel : ViewModelBase
    {
        public MusicStoreViewModel()
        {
            this.WhenAnyValue(x => x.SearchText)
                .Throttle(TimeSpan.FromMilliseconds(400))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(DoSearch!);
        }

        private async void DoSearch(string s)
        {
            IsBusy = true;
            SearchResults.Clear();

            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = _cancellationTokenSource.Token;

            if (!string.IsNullOrWhiteSpace(s))
            {
                var albums = await Album.SearchAsync(s);

                foreach (var album in albums)
                {
                    {
                        var vm = new AlbumViewModel(album);
                        SearchResults.Add(vm);
                    }
                }

                if(!cancellationToken.IsCancellationRequested)
                {
                    LoadCover(cancellationToken);
                }
            IsBusy = false;
            }
        }


        private string? _SearchText;

        private bool _isBusy;

        private AlbumViewModel? _selectedAlbum;

        public ObservableCollection<AlbumViewModel> SearchResults { get; } = new();

        public AlbumViewModel? SelectedAlbum
        {
            get => _selectedAlbum;

            set => this.RaiseAndSetIfChanged(ref _selectedAlbum, value);
        }

        public string? SearchText
        {
            // 隐藏内部属性的方法，不让直接通过set get设置内部的属性，强制通过公开方法设置
            get => _SearchText;
            // RaiseAndSetIfChanged could info.
            set => this.RaiseAndSetIfChanged(ref _SearchText, value);
        }

        public bool IsBusy
        {
            get => _isBusy;
            set => this.RaiseAndSetIfChanged(ref this._isBusy, value);
        }

        private async void LoadCover(CancellationToken cancellationToken)
        {
            foreach (var album in SearchResults.ToList())
            {
                await album.LoadCover();

                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }
            }
        }

        private CancellationTokenSource? _cancellationTokenSource;

        public ReactiveCommand<Unit, AlbumViewModel?> BuyMusicCommand { get; }


    }
}
