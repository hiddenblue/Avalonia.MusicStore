using Avalonia.ReactiveUI;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using Avalonia.MusicStore.ViewModels;
using ReactiveUI;

namespace Avalonia.MusicStore.Views;

public partial class MusicStoreWindow : ReactiveWindow<MusicStoreViewModel>
{ 
    public MusicStoreWindow()
    {
        InitializeComponent();
        this.WhenActivated(d => d(ViewModel!.BuyMusicCommand.Subscribe(Close)));
    }
}