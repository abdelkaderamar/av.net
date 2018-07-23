// Copyright (c) Abdelkader Amar. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.


﻿using System;

namespace Av.API
{
    public class HighUsageException : Exception
    {
        public HighUsageException() : base() {}

        public HighUsageException(string msg) : base(msg)
        {
        }
    }
}
