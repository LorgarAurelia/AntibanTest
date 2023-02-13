using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Antiban
{
    public class EventDTO
    {
        public int MessageId { get; set; }
        public DateTime DateTime { get; set; }
        public DateTime EstimatedSentTime { get; set; }
        public string Phone { get; set; }
        public int Priority { get; set; }
        public bool IsSortedByMainRule { get; set; }

        public EventDTO(EventMessage message)
        {
            MessageId = message.Id;
            DateTime = message.DateTime;
            EstimatedSentTime = message.DateTime;
            Phone = message.Phone;
            Priority = message.Priority;
            IsSortedByMainRule = false;
        }
        public AntibanResult ConvertToAntibanResult()
        {
            return new() { EventMessageId = MessageId, SentDateTime = EstimatedSentTime };
        }

    }
}
