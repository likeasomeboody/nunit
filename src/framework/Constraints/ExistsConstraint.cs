﻿// ***********************************************************************
// Copyright (c) 2014 Charlie Poole
// 
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

#if !NUNITLITE
using System;
using System.IO;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// ExistsConstraint is used to determine if a file or directory exists
    /// </summary>
    public class ExistsConstraint : Constraint
    {
        /// <summary>
        /// If true, the constraint will only check if files exist, not directories
        /// </summary>
        public bool IgnoreDirectories { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExistsConstraint"/> class that
        /// will check files and directories.
        /// </summary>
        public ExistsConstraint(){}

        /// <summary>
        /// Initializes a new instance of the <see cref="ExistsConstraint"/> class that
        /// will only check files if ignoreDirectories is true.
        /// </summary>
        /// <param name="ignoreDirectories">if set to <c>true</c> [ignore directories].</param>
        public ExistsConstraint(bool ignoreDirectories)
        {
            IgnoreDirectories = ignoreDirectories;
        }

        #region Overrides of Constraint

        /// <summary>
        /// The Description of what this constraint tests, for
        /// use in messages and in the ConstraintResult.
        /// </summary>
        public override string Description
        {
            get { return IgnoreDirectories ? "file exists" : "file or directory exists"; }
        }

        /// <summary>
        /// Applies the constraint to an actual value, returning a ConstraintResult.
        /// </summary>
        /// <param name="actual">The value to be tested</param>
        /// <returns>A ConstraintResult</returns>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            if(actual == null)
                throw new ArgumentNullException("actual", "The actual value must be a non-null string" + ErrorSubstring);

            if(actual is string)
            {
                return CheckString(actual);
            }

            var fileInfo = actual as FileInfo;
            if (fileInfo != null)
            {
                return new ConstraintResult(this, actual, fileInfo.Exists);
            }

            var directoryInfo = actual as DirectoryInfo;
            if (!IgnoreDirectories && directoryInfo != null)
            {
                return new ConstraintResult(this, actual, directoryInfo.Exists);
            }
            throw new ArgumentException("The actual value must be a string" + ErrorSubstring, "actual");
        }

        private ConstraintResult CheckString<TActual>(TActual actual)
        {
            var str = actual as string;
            if (String.IsNullOrEmpty(str))
                throw new ArgumentException("The actual value cannot be an empty string", "actual");

            var fileInfo = new FileInfo(str);
            if (IgnoreDirectories)
            {
                return new ConstraintResult(this, actual, fileInfo.Exists);
            }
            var directoryInfo = new DirectoryInfo(str);
            return new ConstraintResult(this, actual, fileInfo.Exists || directoryInfo.Exists);
        }

        private string ErrorSubstring
        {
            get { return IgnoreDirectories ? " or FileInfo" : ", FileInfo or DirectoryInfo"; }
        }

        #endregion
    }
}
#endif