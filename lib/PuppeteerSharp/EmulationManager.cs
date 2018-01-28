﻿using System;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    internal class EmulationManager
    {
        private Session _client;
        private string _injectedTouchScriptId;
        private bool _emulatingMobile;

        public EmulationManager(Session client)
        {
            _client = client;
        }

        internal async Task<bool> EmulateViewport(Session client, ViewPortOptions viewport)
        {
            var mobile = viewport.IsMobile;
            var width = viewport.Width;
            var height = viewport.Height;
            var deviceScaleFactor = viewport.DeviceScaleFactor;
            dynamic screenOrientation = viewport.IsLandscape ?
                new
                {
                    angle = 90,
                    type = ScreenOrientationType.LandscapePrimary
                } :
                new
                {
                    angle = 0,
                    type = ScreenOrientationType.PortraitPrimary
                };

            await Task.WhenAll(new Task[]{
                _client.SendAsync("Emulation.setDeviceMetricsOverride", new
                {
                    mobile,
                    width,
                    height,
                    deviceScaleFactor,
                    screenOrientation
                }),
                _client.SendAsync("Emulation.setTouchEmulationEnabled", new
                {
                    enabled = viewport.HasTouch,
                    configuration = viewport.IsMobile ? "mobile" : "desktop"
                })
            });

            var reloadNeeded = false;
            if (viewport.HasTouch && string.IsNullOrEmpty(_injectedTouchScriptId))
            {
                //TODO: It's not clear what to do here
                /*
                var source = $"({ injectedTouchEventsFunction})()";
                this._injectedTouchScriptId = (await this._client.send('Page.addScriptToEvaluateOnNewDocument', { source })).identifier;
                reloadNeeded = true;
                */
            }
            else if (!viewport.HasTouch && !string.IsNullOrEmpty(_injectedTouchScriptId))
            {
                await _client.SendAsync("Page.removeScriptToEvaluateOnNewDocument", new
                {
                    identifier = _injectedTouchScriptId
                });
                _injectedTouchScriptId = null;
                reloadNeeded = true;
            }

            if (_emulatingMobile != mobile)
            {
                reloadNeeded = true;
            }
            _emulatingMobile = mobile;
            return reloadNeeded;
        }
    }
}