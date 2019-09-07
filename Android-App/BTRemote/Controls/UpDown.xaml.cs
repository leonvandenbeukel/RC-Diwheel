﻿using System;
using System.Collections.Generic;
using RCDiWheel.Models;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RCDiWheel.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UpDown : ContentView
    {
        private readonly Dictionary<long, TouchInfo> _idDictionary = new Dictionary<long, TouchInfo>();
        public int Value { get; private set; } = 0;
        private readonly int _timerDelay = 200;
        private float _canvasWidth = 0;
        private float _canvasHeight = 0;
        private long _id;

        public UpDown()
        {
            InitializeComponent();
            Device.StartTimer(TimeSpan.FromMilliseconds(_timerDelay), OnTimerTick);
        }

        private bool OnTimerTick()
        {
            if (_idDictionary.Count > 0) 
            {
                /*
                            +100%  
                        |
                      -----   0
                        |
                            -100%
                */

                var idPosInfo = _idDictionary[_id];
                var y = idPosInfo.Location.Y;
                var val = _canvasHeight / 2 - y;
                Value = (int)(100 * val / (_canvasHeight / 2));
            }
            else
            {
                Value = 0;
            }

            return true;
        }

        private void CanvasView_OnTouch(object sender, SKTouchEventArgs args)
        {
            switch (args.ActionType)
            {
                case SKTouchAction.Entered:
                    break;
                case SKTouchAction.Pressed:
                    if (args.InContact)
                    {
                        _id = args.Id;
                        _idDictionary.Add(args.Id, new TouchInfo { Location = args.Location });
                    }

                    break;
                case SKTouchAction.Moved:
                    if (_idDictionary.ContainsKey(args.Id))
                    {
                        _idDictionary[args.Id].Location = args.Location;
                    }
                    break;
                case SKTouchAction.Released:
                case SKTouchAction.Cancelled:
                    if (_idDictionary.ContainsKey(args.Id))
                    {
                        _id = 0;
                        _idDictionary.Remove(args.Id);
                    }
                    break;
                case SKTouchAction.Exited:
                    break;
            }

            args.Handled = true;
            CanvasViewUpDown.InvalidateSurface();
        }

        private void CanvasView_OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;
            canvas.Clear(BackgroundColor.ToSKColor());
            var w = canvas.LocalClipBounds.Width;
            var h = canvas.LocalClipBounds.Height;
            var joystickSize = 60;
            var joystickColor = SKColors.DarkSlateBlue;

            if ((int)_canvasHeight == 0)
            {
                _canvasHeight = h;
                _canvasWidth = w;
            }

            var strokeLineStyle = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = SKColors.Purple,
                StrokeWidth = 1,
                PathEffect = SKPathEffect.CreateDash(new float[] { 7, 7 }, 0)
            };

            canvas.DrawLine(w / 2, 0, w / 2, h, strokeLineStyle);
            canvas.DrawLine(0, h / 2, w, h / 2, strokeLineStyle);


            if (_idDictionary.Count == 0)
                canvas.DrawCircle(w / 2, h / 2, joystickSize, new SKPaint { Color = joystickColor, Style = SKPaintStyle.Fill });

            foreach (var key in _idDictionary.Keys)
            {
                var info = _idDictionary[key];

                canvas.DrawCircle(w / 2, info.Location.Y, joystickSize, new SKPaint
                {
                    Color = joystickColor,
                    Style = SKPaintStyle.Fill,
                });
            }
        }
    }
}