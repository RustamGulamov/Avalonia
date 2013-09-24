﻿// -----------------------------------------------------------------------
// <copyright file="HwndSource.cs" company="Steven Kirk">
// Copyright 2013 MIT Licence. See licence.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Avalonia.Direct2D1
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;
    using Avalonia.Direct2D1.Input;
    using Avalonia.Direct2D1.Interop;
    using Avalonia.Direct2D1.Media;
    using Avalonia.Input;
    using Avalonia.Media;
    using Avalonia.Platform;
    using SharpDX.Direct2D1;
    using SharpDX.DXGI;
    using PixelFormat = SharpDX.Direct2D1.PixelFormat;

    public class HwndSource : PlatformPresentationSource
    {
        private UnmanagedMethods.WndProc wndProcDelegate;

        private string className;
        
        private WindowRenderTarget renderTarget;

        [AvaloniaSpecific]
        public HwndSource()
        {
            HwndSourceParameters parameters = new HwndSourceParameters
            {
                PositionX = UnmanagedMethods.CW_USEDEFAULT,
                PositionY = UnmanagedMethods.CW_USEDEFAULT,
                Width = UnmanagedMethods.CW_USEDEFAULT,
                Height = UnmanagedMethods.CW_USEDEFAULT,
                WindowStyle = (int)UnmanagedMethods.WindowStyles.WS_OVERLAPPEDWINDOW
            };

            this.Initialize(parameters);
        }

        public HwndSource(HwndSourceParameters parameters)
        {
            this.Initialize(parameters);
        }

        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Using Win32 naming for consistency.")]
        public HwndSource(int classStyle, int style, int exStyle, int x, int y, string name, IntPtr parent)
            : this(new HwndSourceParameters
            {
                WindowClassStyle = classStyle,
                WindowStyle = style,
                ExtendedWindowStyle = exStyle,
                PositionX = x,
                PositionY = y,
                WindowName = name,
                ParentWindow = parent,
            })
        {
        }

        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Using Win32 naming for consistency.")]
        public HwndSource(int classStyle, int style, int exStyle, int x, int y, int width, int height, string name, IntPtr parent)
            : this(new HwndSourceParameters
            {
                WindowClassStyle = classStyle,
                WindowStyle = style,
                ExtendedWindowStyle = exStyle,
                PositionX = x,
                PositionY = y,
                Width = width,
                Height = height,
                WindowName = name,
                ParentWindow = parent,
            })
        {
        }

        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Using Win32 naming for consistency.")]
        public HwndSource(int classStyle, int style, int exStyle, int x, int y, int width, int height, string name, IntPtr parent, bool adjustSizingForNonClientArea)
            : this(new HwndSourceParameters
            {
                WindowClassStyle = classStyle,
                WindowStyle = style,
                ExtendedWindowStyle = exStyle,
                PositionX = x,
                PositionY = y,
                Width = width,
                Height = height,
                WindowName = name,
                ParentWindow = parent,
            })
        {
        }

        ~HwndSource()
        {
            this.Dispose();
        }

        public override Visual RootVisual { get; set; }

        [AvaloniaSpecific]
        public override Rect BoundingRect
        {
            get
            {
                UnmanagedMethods.RECT rect;
                UnmanagedMethods.GetWindowRect(this.Handle, out rect);
                return new Rect(new Point(rect.left, rect.top), new Point(rect.right, rect.bottom));
            }

            set
            {
                UnmanagedMethods.SetWindowPos(
                    this.Handle,
                    IntPtr.Zero,
                    (int)value.X,
                    (int)value.Y,
                    (int)value.Width,
                    (int)value.Height,
                    0);
            }
        }

        [AvaloniaSpecific]
        public override Size ClientSize
        {
            get
            {
                UnmanagedMethods.RECT rect;
                UnmanagedMethods.GetClientRect(this.Handle, out rect);
                return new Size(rect.right, rect.bottom);
            }
        }

        [AvaloniaSpecific]
        public override DrawingContext CreateDrawingContext()
        {
            return new Direct2D1DrawingContext(
                Direct2D1PlatformInterface.Instance.Direct2DFactory,
                this.renderTarget);
        }

        public override void Dispose()
        {
            if (this.Handle != IntPtr.Zero)
            {
                UnmanagedMethods.DestroyWindow(this.Handle);
                UnmanagedMethods.UnregisterClass(this.className, IntPtr.Zero);
                this.Handle = IntPtr.Zero;
            }

            GC.SuppressFinalize(this);
        }

        public override Point PointToScreen(Point p)
        {
            UnmanagedMethods.POINT result = new UnmanagedMethods.POINT
            {
                X = (int)p.X,
                Y = (int)p.Y,
            };

            UnmanagedMethods.ClientToScreen(this.Handle, ref result);
            return new Point(result.X, result.Y);
        }

        [AvaloniaSpecific]
        public override void Show()
        {
            this.CreateRenderTarget();
            UnmanagedMethods.ShowWindow(this.Handle, 4);
        }

        [AvaloniaSpecific]
        public override void Hide()
        {
            this.CreateRenderTarget();
            UnmanagedMethods.ShowWindow(this.Handle, 0);
        }

        private void Initialize(HwndSourceParameters parameters)
        {
            // Ensure that the delegate doesn't get garbage collected by storing it as a field.
            this.wndProcDelegate = new UnmanagedMethods.WndProc(this.WndProc);

            this.className = Guid.NewGuid().ToString();

            UnmanagedMethods.WNDCLASSEX wndClassEx = new UnmanagedMethods.WNDCLASSEX
            {
                cbSize = Marshal.SizeOf(typeof(UnmanagedMethods.WNDCLASSEX)),
                style = parameters.WindowClassStyle,
                lpfnWndProc = this.wndProcDelegate,
                hInstance = Marshal.GetHINSTANCE(this.GetType().Module),
                hCursor = UnmanagedMethods.LoadCursor(IntPtr.Zero, (int)UnmanagedMethods.Cursor.IDC_ARROW),
                hbrBackground = (IntPtr)5,
                lpszClassName = this.className,
            };

            ushort atom = UnmanagedMethods.RegisterClassEx(ref wndClassEx);

            if (atom == 0)
            {
                throw new Win32Exception();
            }

            this.Handle = UnmanagedMethods.CreateWindowEx(
                parameters.ExtendedWindowStyle,
                atom,
                parameters.WindowName,
                parameters.WindowStyle,
                parameters.PositionX,
                parameters.PositionY,
                parameters.Width,
                parameters.Height,
                parameters.ParentWindow,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero);

            if (this.Handle == IntPtr.Zero)
            {
                throw new Win32Exception();
            }
        }

        private void CreateRenderTarget()
        {
            Size clientSize = this.ClientSize;
            SharpDX.Direct2D1.Factory d2dFactory = Direct2D1PlatformInterface.Instance.Direct2DFactory;
            
            this.renderTarget = new WindowRenderTarget(
                d2dFactory,
                new RenderTargetProperties
                {
                    PixelFormat = new PixelFormat(Format.Unknown, AlphaMode.Premultiplied)                    
                },
                new HwndRenderTargetProperties
                {
                    Hwnd = this.Handle,
                    PixelSize = new SharpDX.DrawingSize((int)clientSize.Width, (int)clientSize.Height),
                });
        }

        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Using Win32 naming for consistency.")]
        private IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            Win32KeyboardDevice keyboard = Win32KeyboardDevice.Instance;
            Win32MouseDevice mouse = Win32MouseDevice.Instance;

            mouse.SetActiveSource(this);
            keyboard.SetActiveSource(this);

            // Only update the mouse cursor pos each time we receive a message; otherwise when 
            // debugging the mouse class will report the actual mouse position which makes things
            // a lot more difficult. Do the same for the keyboard state.
            mouse.UpdateCursorPos();
            keyboard.UpdateKeyStates();

            switch ((UnmanagedMethods.WindowsMessage)msg)
            {
                case UnmanagedMethods.WindowsMessage.WM_DESTROY:
                    this.OnClosed();
                    break;

                case UnmanagedMethods.WindowsMessage.WM_KEYDOWN:
                    InputManager.Current.ProcessInput(
                        new RawKeyEventArgs(
                            keyboard, 
                            RawKeyEventType.KeyDown, 
                            KeyInterop.KeyFromVirtualKey((int)wParam)));
                    break;

                case UnmanagedMethods.WindowsMessage.WM_LBUTTONDOWN:
                    InputManager.Current.ProcessInput(new RawMouseEventArgs(mouse, RawMouseEventType.LeftButtonDown));
                    break;

                case UnmanagedMethods.WindowsMessage.WM_LBUTTONUP:
                    InputManager.Current.ProcessInput(new RawMouseEventArgs(mouse, RawMouseEventType.LeftButtonUp));
                    break;

                case UnmanagedMethods.WindowsMessage.WM_MOUSEMOVE:
                    InputManager.Current.ProcessInput(new RawMouseEventArgs(mouse, RawMouseEventType.Move));
                    break;

                case UnmanagedMethods.WindowsMessage.WM_SIZE:
                    if (this.renderTarget != null)
                    {
                        this.renderTarget.Resize(new SharpDX.DrawingSize((int)lParam & 0xffff, (int)lParam >> 16));
                    }

                    this.OnResized();
                    return IntPtr.Zero;
            }

            return UnmanagedMethods.DefWindowProc(hWnd, msg, wParam, lParam);
        }
    }
}
