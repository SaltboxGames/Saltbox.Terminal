/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using UnityEngine;

namespace Saltbox.Terminal
{
    public delegate void TerminalEvent(ITerminalControl sender, TerminalEventArgs args);

    public struct TerminalEventArgs
    {
        public string content;
    }

    public interface ITerminalControl
    {
        event TerminalEvent OnInputSubmit;
        string Name { get; }

        void Clear();
        void Write(string content, Color color);
        void WriteLine(string content, Color color);
    }
}
