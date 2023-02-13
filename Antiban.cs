namespace Antiban
{
    public class Antiban //Prev Class Name Antiban
    {
        private List<EventDTO> _incomedMessages = new();
        /// <summary>
        /// Добавление сообщений в систему, для обработки порядка сообщений
        /// </summary>
        /// <param name="eventMessage"></param>
        public void PushEventMessage(EventMessage eventMessage)
        {

            _incomedMessages.Add(new(eventMessage));

        }
        /// <summary>
        /// Вовзращает порядок отправок сообщений
        /// </summary>
        /// <returns></returns>
        public List<AntibanResult> GetResult()
        {
            SortByFirstRule();

            SortMessagePerPhoneNumber();

            SortMessageForMailing();

            _incomedMessages.Sort((x, y) => DateTime.Compare(x.EstimatedSentTime, y.EstimatedSentTime));

            var messagesOrder = new List<AntibanResult>();
            foreach (var item in _incomedMessages)
            {
                messagesOrder.Add(item.ConvertToAntibanResult());
            }

            return messagesOrder;
        }
        private void SortByFirstRule()
        {
            _incomedMessages.Sort((x, y) => DateTime.Compare(x.DateTime, y.DateTime));

            DateTime compareTime = new();
            for (int i = 0; i < _incomedMessages.Count; i++)
            {
                if (_incomedMessages[i].IsSortedByMainRules)
                    continue;

                if (i == 0)
                {
                    compareTime = _incomedMessages[i].EstimatedSentTime;
                    continue;
                }

                if ((_incomedMessages[i].EstimatedSentTime - compareTime).TotalSeconds < SortRules.IntervalPerMessageGlobal.TotalSeconds)
                {
                    _incomedMessages[i].EstimatedSentTime = compareTime.AddSeconds(SortRules.IntervalPerMessageGlobal.TotalSeconds);
                }
                compareTime = _incomedMessages[i].EstimatedSentTime;
            }
        }
        private void SortMessagePerPhoneNumber()
        {
            foreach (var message in _incomedMessages)
            {
                var messagePerNumber = _incomedMessages.Where(m => m.Phone == message.Phone && m.MessageId < message.MessageId && m.EstimatedSentTime <= message.EstimatedSentTime).LastOrDefault();

                if (messagePerNumber == null)
                    continue;

                if ((message.EstimatedSentTime - messagePerNumber.EstimatedSentTime).TotalSeconds < SortRules.IntervalToNumber.TotalSeconds)
                {
                    message.EstimatedSentTime = messagePerNumber.EstimatedSentTime.AddMinutes(SortRules.IntervalToNumber.TotalMinutes);
                    message.IsSortedByMainRules = true;
                }
            }
        }
        private void SortMessageForMailing()
        {
            foreach (var message in _incomedMessages)
            {
                if (message.Priority == 0)
                    continue;

                var mailing = _incomedMessages.Where(m => m.Priority == 1 && m.Phone == message.Phone && m.MessageId < message.MessageId).LastOrDefault();

                if (mailing == null)
                    continue;

                if ((message.EstimatedSentTime - mailing.EstimatedSentTime).TotalSeconds < SortRules.IntervalToMailing.TotalSeconds)
                {
                    message.EstimatedSentTime = mailing.EstimatedSentTime.AddHours(SortRules.IntervalToMailing.TotalHours);
                    message.IsSortedByMainRules = true;
                }
            }
        }
    }
}
