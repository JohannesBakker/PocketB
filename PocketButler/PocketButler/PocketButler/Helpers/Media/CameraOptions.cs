﻿using System;


namespace PocketButler.Helpers.Media
{
    
    public class CameraOptions : AbstractOptions {
        public CameraDevice Camera { get; set; }


        public CameraOptions() {
            this.Camera = CameraDevice.Rear;
        }
    }
}
