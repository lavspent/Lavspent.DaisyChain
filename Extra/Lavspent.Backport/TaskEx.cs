/*
    Copyright(c) 2017 Petter Labråten. All rights reserved.

    The MIT License(MIT)

    Permission is hereby granted, free of charge, to any person obtaining a
    copy of this software and associated documentation files (the "Software"),
    to deal in the Software without restriction, including without limitation
    the rights to use, copy, modify, merge, publish, distribute, sublicense,
    and/or sell copies of the Software, and to permit persons to whom the
    Software is furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in
    all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
    FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
    DEALINGS IN THE SOFTWARE.
*/

using System.Threading.Tasks;

namespace Lavspent.Backport
{
    /// <summary>
    /// 
    /// </summary>
    public static class TaskEx
    {
        #region CompletedTask

        private static Task _completedTask;

        static TaskEx()
        {
            // Create a completed task. 
            // TODO: Is this enought? The ref source seems to suggest that we 
            // need to cancel any attempts of disposing....?
            // var tcs = new TaskCompletionSource<object>();
            // tcs.SetResult(default(object));
            //_completedTask = tcs.Task;

            // or what about simply this?
            _completedTask = Task.FromResult<object>(null);
        }

        /// <summary>
        /// A cached completed task.
        /// </summary>
        public static Task CompletedTask
        {
            get
            {
                return _completedTask;
            }
        }

        #endregion
    }
}
