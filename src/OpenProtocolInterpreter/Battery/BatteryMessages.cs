using OpenProtocolInterpreter.Messages;
using System;
using System.Collections.Generic;

namespace OpenProtocolInterpreter.Battery
{
    /// <summary>
    /// Template for <see cref="IBattery"/> implementers.
    /// </summary>
    internal class BatteryMessages : MessagesTemplate
    {
        public BatteryMessages() : base()
        {
            _templates = new Dictionary<int, CompiledInstance<Mid>>()
            {
                { Mid0800.MID, new CompiledInstance<Mid>(typeof(Mid0800)) },
                { Mid0801.MID, new CompiledInstance<Mid>(typeof(Mid0801)) },
                { Mid0802.MID, new CompiledInstance<Mid>(typeof(Mid0802)) },
                { Mid0803.MID, new CompiledInstance<Mid>(typeof(Mid0803)) },
                { Mid0804.MID, new CompiledInstance<Mid>(typeof(Mid0804)) }
            };
        }

        public BatteryMessages(IEnumerable<Type> selectedMids) : this()
        {
            FilterSelectedMids(selectedMids);
        }

        public BatteryMessages(InterpreterMode mode) : this()
        {
            FilterSelectedMids(mode);
        }

        public override bool IsAssignableTo(int mid) => mid > 799 && mid < 805;
    }
}
