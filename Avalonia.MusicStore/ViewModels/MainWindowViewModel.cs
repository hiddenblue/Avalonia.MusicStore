using ReactiveUI;
using System.Windows.Input;
using System;
using System.Reactive.Linq;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.MusicStore.Models;
using System.Reactive.Concurrency;

namespace Avalonia.MusicStore.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {


        public MainWindowViewModel()
        {
            RxApp.MainThreadScheduler.Schedule(LoadAlbums);

            ShowDialog = new Interaction<MusicStoreViewModel, AlbumViewModel?>();

            BuyMusicCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var store = new MusicStoreViewModel();

                var result = await ShowDialog.Handle(store);

                if(result != null)
                    {
                        Albums.Add(result);
                        await result.SaveToDiskAsync();
                    }
            });
        }

        public ICommand BuyMusicCommand { get; }

        // 主窗口创建一个交互的声明
        public Interaction<MusicStoreViewModel, AlbumViewModel?> ShowDialog {  get; }

        public ObservableCollection<AlbumViewModel> Albums { get; } = new();

        private async void LoadAlbums()
        {
            var albums = (await Album.LoadCacheAsync()).Select(x => new AlbumViewModel(x));

            foreach (var album in albums)
            {
                Albums.Add(album);
            }


            foreach (var album in Albums.ToList())
            {
                await album.LoadCover();
            }
        }
    }
}
