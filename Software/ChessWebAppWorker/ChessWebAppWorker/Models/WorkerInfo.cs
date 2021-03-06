﻿using System;

namespace ChessWebAppWorker
{
    public class WorkerInfo
    {
        public byte[] GamePosition { get; set; }
        public byte Color { get; set; }
        public int WorkerID { get; set; }

        public WorkerInfo(byte[] gamePosition, byte color, int workerID)
        {
            GamePosition = new byte[64];
            Array.Copy(gamePosition, GamePosition, gamePosition.Length);
            Color = color;
            WorkerID = workerID;
        }
    }
}
