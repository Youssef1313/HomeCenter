﻿using System;
using System.Collections.Generic;
using System.Text;

namespace HomeCenter.Model.Adapters.Sony
{
    public class BraviaApiException : Exception
    {
        public int ErrorId { get; }

        public BraviaApiException(int errorId, string message)
            : base(message)
        {
            this.ErrorId = errorId;
        }
    }
}