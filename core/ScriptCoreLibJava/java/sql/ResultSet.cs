﻿using ScriptCoreLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace java.sql
{
    // http://docs.oracle.com/javase/1.4.2/docs/api/java/sql/ResultSet.html
    [Script(IsNative = true)]
    public interface ResultSet
    {
        bool next();

        string getString(int value);
        int getInt(int value);
        int findColumn(string value);
    }
}
