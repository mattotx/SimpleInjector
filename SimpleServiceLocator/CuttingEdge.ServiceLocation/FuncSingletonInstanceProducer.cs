﻿#region Copyright (c) 2010 S. van Deursen
/* The SimpleServiceLocator library is a simple but complete implementation of the CommonServiceLocator 
 * interface.
 * 
 * Copyright (C) 2010 S. van Deursen
 * 
 * To contact me, please visit my blog at http://www.cuttingedge.it/blogs/steven/ or mail to steven at 
 * cuttingedge.it.
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and 
 * associated documentation files (the "Software"), to deal in the Software without restriction, including 
 * without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
 * copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the 
 * following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all copies or substantial 
 * portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT 
 * LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO 
 * EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER 
 * IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE 
 * USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion

using System;

using Microsoft.Practices.ServiceLocation;

namespace CuttingEdge.ServiceLocation
{
    /// <summary>Ensures that the wrapped delegate will only be executed once.</summary>
    /// <typeparam name="T">The interface or base type that can be used to retrieve instances.</typeparam>
    internal sealed class FuncSingletonInstanceProducer<T> : IInstanceProducer where T : class
    {
        private Func<T> singleInstanceCreator;
        private bool instanceCreated;
        private T instance;

        internal FuncSingletonInstanceProducer(Func<T> singleInstanceCreator)
        {
            this.singleInstanceCreator = singleInstanceCreator;
        }

        /// <summary>Produces an instance.</summary>
        /// <returns>An instance.</returns>
        public object GetInstance()
        {
            // We use a lock to prevent the delegate to be called more than once during the lifetime of
            // the application. We use a double checked lock to prevent the lock statement from being 
            // called again after the instance was created.
            if (!this.instanceCreated)
            {
                // We can take a lock on this, because this class is private.
                lock (this)
                {
                    if (!this.instanceCreated)
                    {
                        this.instance = this.singleInstanceCreator();
                        this.instanceCreated = true;

                        // Remove the reference to the delegate; it is not needed anymore.
                        this.singleInstanceCreator = null;
                    }
                }
            }

            return this.instance;
        }
    }
}