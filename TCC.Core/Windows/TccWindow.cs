﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using TCC.Controls;
using TCC.ViewModels;

namespace TCC.Windows
{
    public class TccWindow : Window, INotifyPropertyChanged
    {
        protected IntPtr _handle;
        protected WindowSettings _settings;
        protected WindowButtons _b;
        protected UIElement _c;
        DispatcherTimer _t;
        DoubleAnimation _showButtons;
        DoubleAnimation _hideButtons;
        protected bool _ignoreSize;
        protected bool clickThru;
        public bool ClickThru
        {
            get { return clickThru; }
            set
            {
                clickThru = value;

                if (clickThru) FocusManager.MakeTransparent(_handle);
                else FocusManager.UndoTransparent(_handle);

                NPC();
            }
        }
        public WindowSettings WindowSettings => _settings;

        protected void InitWindow(WindowSettings ws, bool canClickThru = true, bool canHide = true, bool ignoreSize = true)
        {
            Topmost = true;
            _settings = ws;
            _settings.NotifyWindowSafeClose += CloseWindowSafe;
            _settings.PropertyChanged += _settings_PropertyChanged;
            Left = ws.X;
            Top = ws.Y;
            if (!ignoreSize)
            {
                if (ws.H != 0) Height = ws.H;
                if (ws.W != 0) Width = ws.W;
            }
            _ignoreSize = ignoreSize;
            SetVisibility(ws.Visible);
            //Visibility = ws.Visible ? Visibility.Visible : Visibility.Hidden;
            SetClickThru(ws.ClickThruMode == ClickThruMode.Always);
            if (_settings.AutoDim) AnimateContentOpacity(_settings.DimOpacity);
            if (!WindowManager.IsTccVisible) AnimateContentOpacity(0);

            WindowManager.TccVisibilityChanged += OpacityChange;
            WindowManager.TccDimChanged += OpacityChange;
            SizeChanged += TccWindow_SizeChanged;
            Closed += TccWindow_Closed;
            Loaded += TccWindow_Loaded;

            if (_b == null) return;

            _hideButtons = new DoubleAnimation(0, TimeSpan.FromMilliseconds(1000));
            _showButtons = new DoubleAnimation(1, TimeSpan.FromMilliseconds(150));

            _t = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(2) };
            _t.Tick += (s, ev) =>
            {
                _t.Stop();
                if (this.IsMouseOver) return;
                _b.BeginAnimation(OpacityProperty, _hideButtons);
            };

            MouseEnter += (s, ev) => _b.BeginAnimation(OpacityProperty, _showButtons);
            MouseLeave += (s, ev) => _t.Start();
            _b.MouseLeftButtonDown += Drag;
        }

        private void _settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_settings.ClickThruMode))
            {
                switch (_settings.ClickThruMode)
                {
                    case ClickThruMode.Never:
                        FocusManager.UndoTransparent(_handle);
                        break;
                    case ClickThruMode.Always:
                        FocusManager.MakeTransparent(_handle);
                        break;
                    case ClickThruMode.WhenDim:
                        if (WindowManager.IsTccDim) FocusManager.MakeTransparent(_handle);
                        else FocusManager.UndoTransparent(_handle);
                        break;
                    case ClickThruMode.WhenUndim:
                        if (WindowManager.IsTccDim) FocusManager.UndoTransparent(_handle);
                        else FocusManager.MakeTransparent(_handle);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else if (e.PropertyName == nameof(_settings.Scale))
            {
                Dispatcher.Invoke(() =>
                {
                    var vm = (TccWindowViewModel)DataContext;
                    vm.GetDispatcher().Invoke(() => vm.Scale = _settings.Scale);
                });
            }
            else if (e.PropertyName == nameof(_settings.Visible))
            {
                SetVisibility(_settings.Visible);
            }
        }

        protected void TccWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _handle = new WindowInteropHelper(this).Handle;
            FocusManager.MakeUnfocusable(_handle);
            FocusManager.HideFromToolBar(_handle);

            if (!_settings.Enabled) CloseWindowSafe();
        }

        private void TccWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            CheckBounds();
            if (_ignoreSize) return;
            _settings.W = ActualWidth;
            _settings.H = ActualHeight;
            SettingsManager.SaveSettings();
        }

        private void TccWindow_Closed(object sender, EventArgs e)
        {
            //Dispatcher.InvokeShutdown();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NPC([CallerMemberName] string p = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
        }
        private void OpacityChange(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsTccVisible")
            {
                if (WindowManager.IsTccVisible)
                {
                    if (WindowManager.IsTccDim && _settings.AutoDim)
                    {
                        AnimateContentOpacity(_settings.DimOpacity);
                    }
                    else
                    {
                        AnimateContentOpacity(1);
                    }
                }
                else
                {
                    if (_settings.ShowAlways) return;
                    AnimateContentOpacity(0);
                }
            }

            //TODO: rework dim/undim and clickthru logic
            if (e.PropertyName == "IsTccDim")
            {
                if (!WindowManager.IsTccVisible) return;
                if (!_settings.AutoDim) return;

                AnimateContentOpacity(WindowManager.IsTccDim ? _settings.DimOpacity : 1);
                if (_settings.ClickThruMode == ClickThruMode.WhenUndim)
                {
                    SetClickThru(!WindowManager.IsTccDim);
                }
                else if (_settings.ClickThruMode == ClickThruMode.WhenDim)
                {
                    SetClickThru(WindowManager.IsTccDim);
                }
            }
        }
        public void SetClickThru(bool t)
        {
            ClickThru = t;
        }
        public void SetVisibility(Visibility v)
        {
            if (!Dispatcher.Thread.IsAlive)
            {
                return;
            }
            Dispatcher.Invoke(() =>
            {
                Visibility = v;
                NPC("Visibility");
            });
        }
        public void SetVisibility(bool v)
        {
            if (!Dispatcher.Thread.IsAlive)
            {
                return;
            }
            Dispatcher.Invoke(() =>
            {
                Visibility = !v ? Visibility.Visible : Visibility.Collapsed; // meh ok
                Visibility = v ? Visibility.Visible : Visibility.Collapsed;
                NPC("Visibility");
            });
        }

        public void AnimateContentOpacity(double opacity)
        {
            if (_c == null) return;
            Dispatcher.InvokeIfRequired(() =>
            {
                //var grid = ((Grid)this.Content);
                _c.BeginAnimation(OpacityProperty, new DoubleAnimation(opacity, TimeSpan.FromMilliseconds(250)));
            }, System.Windows.Threading.DispatcherPriority.DataBind);
        }
        public void RefreshTopmost()
        {
            Dispatcher.InvokeIfRequired(() => { Topmost = false; Topmost = true; }, System.Windows.Threading.DispatcherPriority.DataBind);
        }
        public void RefreshSettings(WindowSettings ws)
        {
            _settings = ws;
        }

        protected void Drag(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (!_ignoreSize) ResizeMode = ResizeMode.NoResize;
                DragMove();
                CheckBounds();
                if (!_ignoreSize) ResizeMode = ResizeMode.CanResize;
                var screen = Screen.FromHandle(new WindowInteropHelper(this).Handle);
                var source = PresentationSource.FromVisual(this);
                if (source?.CompositionTarget == null) return;
                var m = source.CompositionTarget.TransformToDevice;
                var dx = m.M11;
                var dy = m.M22;
                var newLeft = Left * dx;
                var newTop = Top * dx;
                _settings.X = newLeft / dx;
                _settings.Y = newTop / dy;

                SettingsManager.SaveSettings();
            }
            catch (Exception) { }
        }

        private void CheckBounds()
        {
            if (Left < 0) Left = 0;
            if ((Left + ActualWidth) > Screen.PrimaryScreen.WorkingArea.Width)
            {
                Left = Screen.PrimaryScreen.Bounds.Width - ActualWidth;
            }
            if ((Top + ActualHeight) > Screen.PrimaryScreen.WorkingArea.Height)
            {
                Top = Screen.PrimaryScreen.Bounds.Height - ActualHeight;
            }
        }

        public void CloseWindowSafe()
        {
            if (Dispatcher.CheckAccess())
                Close();
            else
                Dispatcher.Invoke(DispatcherPriority.Normal, new ThreadStart(Close));
        }
    }
}