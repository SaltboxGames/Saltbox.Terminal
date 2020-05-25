/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using CommandLine;
using UnityEngine;

namespace Saltbox.Terminal.Commands
{
    class ClearCommand : ICommand
    {
        [Option('a', "all", HelpText="Clear All Terminals")]
        public bool clearAll { get; set; }

        public bool Invoke(ITerminalControl sender, CommandRunner runner)
        {
            if (clearAll)
            {
                runner.Clear();
                return true;
            }
            sender.Clear();
            return true;
        }
    }
}
